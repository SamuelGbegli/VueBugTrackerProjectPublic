<!--Page for searching for and filtering projects-->
<template>
  <div class="q-gutter-md">
  <!--Shows title and button to change filter-->
  <div class="row">
    <h3>Search</h3>
    <QSpace/>
    <QBtn @click="showFilterDialog = true" label="Change filter"/>
  </div>
  <!--Visible when projects are found or being loaded-->
  <div class="q-gutter-md" v-if="!statusCode || statusCode === 200">
  <div v-if="!!statusCode">
    <h6>Total projects: {{ projects.totalProjects }}</h6>
  </div>
    <div v-if="projects.totalProjects > 0">
      <!--Shows list of projects if any were found-->
      <div class="row q-gutter-md">
      <ProjectPreview v-for="x in projects.projects" :-project="x" v-bind:key="(x as ProjectPreviewViewModel).id"/>
    </div>
    <div class="row">
        <QSpace/>
        <QPagination v-model="currentPage"
                   :min="1"
                   :max="numberOfPages"
                   @update:model-value="changePage"
                   input />
      </div>
    </div>
    <!--Visible if there are no projects found-->
    <div v-else-if="!!statusCode">
        <h6>No projects were found.</h6>
      </div>
      <QInnerLoading
        :showing="!statusCode"
        style="width: 100%; height: 100%"/>
    </div>
    <!--Visible if the client does not get a response from the server
      or somehow goes to a page greater than the filter's maximum-->
    <div v-else>
      <ErrorBanner :prompt-type="statusCode === 500? ErrorPromptType.ReloadButton : ErrorPromptType.GoBackButtonOnly"
      :message="statusCode === 500? 'There was an error connecting with the server.' : 'The page you requested does not exist.'"/>
    </div>
  </div>
    <QDialog v-model="showFilterDialog">
      <QCard style="min-width: 400px;">
        <QCardSection>
        <div class="row">
          <h6>Filter</h6>
          <QSpace/>
          <QBtn icon="close" flat round dense v-close-popup/>
        </div>
        </QCardSection>
        <QForm @submit="submitFilter">
          <!--Section for project query and type of projects-->
          <QCardSection>
            <QInput v-model="filterQuery.query" label="Query" stack-label/>
            <QSelect :options="projectTypeValues" v-model="filterQuery.projectType" label="Project types" stack-label/>
          </QCardSection>
          <!--Section for setting filter date range-->
          <QCardSection>
            <QSelect :options="dateSearchValues" v-model="filterQuery.dateSearch" label="Date search" stack-label/>
            <QInput v-model="filterQuery.dateFrom" type="date" label="From" stack-label/>
            <QInput v-model="filterQuery.dateEnd" type="date" label="To" stack-label/>
          </QCardSection>
          <!--Section for sorting projects-->
          <QCardSection>
            <QSelect :options="sortTypeValues" v-model="filterQuery.sortType" label="Sort type" stack-label/>
            <QSelect :options="sortOrderValues" v-model="filterQuery.sortOrder" label="Sort order" stack-label/>
          </QCardSection>
          <QCardActions align="right">
            <QBtn @click="resetFilter" label="Reset"/>
            <QBtn type="submit" label="Submit"/>
          </QCardActions>
        </QForm>
      </QCard>
    </QDialog>
</template>
<script setup lang="ts">
import FilterDTO from '@/classes/DTOs/FilterDTO';
import ErrorBanner from '@/components/ErrorBanner.vue';
import ProjectPreview from '@/components/ProjectPreview.vue';
import DateSearch from '@/enumConsts/DateSearch';
import ErrorPromptType from '@/enumConsts/ErrorPromptType';
import ProjectType from '@/enumConsts/ProjectType';
import SortOrder from '@/enumConsts/SortOrder';
import SortType from '@/enumConsts/SortType';
import ProjectContainer from '@/viewmodels/ProjectContainer'
import type ProjectPreviewViewModel from '@/viewmodels/ProjectPreviewViewModel';
import axios, { AxiosError } from 'axios';
import { onBeforeMount, ref } from 'vue';

//Projects visible by the user
const projects = ref(new ProjectContainer());

//The page the user is on
const currentPage = ref(1);

//The number of pages the pagination control will see
const numberOfPages = ref(1);

//The status code of the request to get projects
const statusCode = ref();

//Stores the user's current filter and sort options
const filterDTO = ref(new FilterDTO());

//Shows the dialog to change the filter
const showFilterDialog = ref(false);

//Values for project type dropdown
const projectTypeValues = ["All projects", "Projects with no open bugs", "Projects with open bugs"];

//Values for date search type dropdown
const dateSearchValues = ["Date created", "Last updated"];

//Values for sort type dropdown
const sortTypeValues = ["Name", "Date created", "Last updated"];

//Values for sort order dropdown
const sortOrderValues = ["Ascending", "Descending"];

//Stores values in the filter form
const filterQuery = ref({
  query: "",
  projectType: projectTypeValues[ProjectType.All],
  dateSearch: dateSearchValues[DateSearch.CreatedDate],
  dateFrom: null,
  dateEnd: null,
  sortType: sortTypeValues[SortType.Name],
  sortOrder: sortOrderValues[SortOrder.Ascending]
});

onBeforeMount(async () =>{
  getProjects();
})

//Loads project from server
async function getProjects() {
  try{

    //Gets projects from server
    const response = await axios.post(`/projects/search`, filterDTO.value);

    //Assigns project view model objects
    projects.value = Object.assign(new ProjectContainer(), response.data);

    //Sets the maximum value of the pagination control
    numberOfPages.value = Math.ceil(projects.value.totalProjects / 30);

    //Sets status code
    //If no projects are found, sets a 404 error message
    if(projects.value.projects.length === 0 && filterDTO.value.pageNumber != 1) statusCode.value = 404;
    //Otherwise sets 200 from the server
    else statusCode.value = response.status;

  }
  catch(ex){
    //Sets status code error
    const error = ex as AxiosError;
    statusCode.value = error.status;
  }
}

//Resets the filter's values
function resetFilter(){
  filterQuery.value = {
    query: "",
    projectType: projectTypeValues[ProjectType.All],
    dateSearch: dateSearchValues[DateSearch.CreatedDate],
    dateFrom: null,
    dateEnd: null,
    sortType: sortTypeValues[SortType.Name],
    sortOrder: sortOrderValues[SortOrder.Ascending]
  }
}

//Submits the form
async function submitFilter() {
  showFilterDialog.value = false;

  //Saves values to DTO
  filterDTO.value.query = filterQuery.value.query;
  filterDTO.value.projectType = projectTypeValues.indexOf(filterQuery.value.projectType);
  filterDTO.value.dateSearch = dateSearchValues.indexOf(filterQuery.value.dateSearch);
  filterDTO.value.dateFrom = filterQuery.value.dateFrom === "" ? null : filterQuery.value.dateFrom;
  filterDTO.value.dateEnd = filterQuery.value.dateEnd === "" ? null : filterQuery.value.dateEnd;
  filterDTO.value.sortType = sortTypeValues.indexOf(filterQuery.value.sortType);
  filterDTO.value.sortOrder = sortOrderValues.indexOf(filterQuery.value.sortOrder);

  //Sets filter to get first set of projects
  filterDTO.value.pageNumber = 1;
  await getProjects();
}

//Changes the page of projects returned from the server
async function changePage(){
  filterDTO.value.pageNumber = currentPage.value;
  await getProjects();
}

</script>
