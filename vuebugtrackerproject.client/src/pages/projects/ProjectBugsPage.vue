<template>
  <div class="q-gutter-md">
  <div class="row">
      <h6>Number of bugs: {{ bugContainer.numberOfBugs }}</h6>
      <QSpace/>
      <QBtn @click="openDialog" label="Filter"/>
      <QBtn v-if="project.ownerID === authStore.getUserID()" :to="`/project/${route.params.projectId}/bugs/add`"
      label="Add bug"/>
  </div>
  <div v-if="!!bugContainer && bugContainer.numberOfBugs > 0">
  <!--Table to show bugs-->
    <QMarkupTable>
        <thead>
          <tr>
            <th>Description</th>
            <th>Last updated</th>
            <th>Created by</th>
            <th>Severity</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="x in bugContainer.bugPreviews" v-bind:key="(x as BugPreviewViewModel).id">
            <td>{{ (x as BugPreviewViewModel).summary }}</td>
            <td>{{ formatDate((x as BugPreviewViewModel).dateModified) }}</td>
            <td>
              <UserIcon :username="(x as BugPreviewViewModel).creatorName" :icon="(x as BugPreviewViewModel).creatorIcon"/>
            </td>
            <td>
              <QChip :color="getChipColour(x.severity)">
                {{ (x as BugPreviewViewModel).severity }}
              </QChip>
            </td>
            <td>
              <QChip :color="getChipColour(x.status)">
                {{ (x as BugPreviewViewModel).status }}
              </QChip>
            </td>
            <td>
              <QBtnDropdown
              split
                label="View"
                :to="`/bug/${x.id}`">
                <QList>
                  <QItem clickable v-close-popup :to="`/bug/${x.id}/discussion`">
                    <QItemSection>
                      <QItemLabel>Discussion</QItemLabel>
                    </QItemSection>
                  </QItem>
                  <QItem clickable v-close-popup :to="`/bug/${x.id}/settings`"
                  v-if="(x as BugPreviewViewModel).creatorID === authStore.getUserID() || project.ownerID === authStore.getUserID()">
                    <QItemSection>
                      <QItemLabel>Settings</QItemLabel>
                    </QItemSection>
                  </QItem>
                </QList>
              </QBtnDropdown>
            </td>
          </tr>
        </tbody>
      </QMarkupTable>
      <!--Pagination control for showing different bugs-->
      <div class="row">
      <QSpace/>
        <QPagination v-model="bugContainer.currentPage"
        :min="1"
        :max="numberOfPages"
        input
        @update:model-value="onPageUpdate"/>
      </div>
      <QInnerLoading
      :showing="loading"
      label="Please wait..."/>
  </div>
  <!--Message if project has no bugs-->
  <h5 v-else>This project has no bugs.</h5>
  </div>
  <!--Dialog for filtering bug-->
  <QDialog v-model="showFilterDialog">
      <QCard style="min-width: 400px;">
        <QCardSection>
        <div class="row">
          <h6>Filter</h6>
          <QSpace/>
          <QBtn icon="close" flat round dense v-close-popup/>
        </div>
        </QCardSection>
        <QForm @submit="onSubmit">
          <!--Section for bug summary and creator-->
          <QCardSection>
            <QInput v-model="filterFormValues.summary" label="Summary" stack-label/>
            <QInput v-model="filterFormValues.creatorName" label="Creator username" stack-label/>
          </QCardSection>
          <!--Section for bug status and severity-->
          <QCardSection>
          <span>Severity</span>
          <br/>
            <QOptionGroup
            v-model="filterFormValues.severityValues"
            :options="severityValues"
            type="checkbox"
            inline/>
          <span>Status</span>
          <br/>
            <QOptionGroup
            v-model="filterFormValues.statusValues"
            :options="statusValues"
            type="checkbox"
            inline/>
          </QCardSection>
          <!--Section for setting filter date range-->
          <QCardSection>
            <QSelect :options="dateSearchValues" v-model="filterFormValues.dateSearch" label="Date search" stack-label/>
            <QInput v-model="filterFormValues.dateFrom" type="date" label="From" stack-label/>
            <QInput v-model="filterFormValues.dateTo" type="date" label="To" stack-label/>
          </QCardSection>
          <!--Section for sorting bugs-->
          <QCardSection>
            <QSelect :options="sortTypeValues" v-model="filterFormValues.sortType" label="Sort type" stack-label/>
            <QSelect :options="sortOrderValues" v-model="filterFormValues.sortOrder" label="Sort order" stack-label/>
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
import UserIcon from '@/components/UserIcon.vue';
import BugPreviewViewModel from '@/viewmodels/BugPreviewViewModel';
import axios from 'axios';
import { onBeforeMount, ref } from 'vue';
import { useRoute } from 'vue-router';
import { useAuthStore } from '@/stores/AuthStore';
import ProjectViewModel from '@/viewmodels/ProjectViewModel';
import formatDate from '@/classes/helpers/FormatDate';
import BugFilterDTO from '@/classes/DTOs/BugFilterDTO';
import DateSearch from '@/enumConsts/DateSearch';
import SortType from '@/enumConsts/SortType';
import SortOrder from '@/enumConsts/SortOrder';
import Severity from '@/enumConsts/Severity';
import Status from '@/enumConsts/Status';
import BugPreviewContainer from '@/viewmodels/BugPreviewContainer';

//The number of pages of project bugs in the backend, in groups of 20
const numberOfPages = ref(5);

//The bug container returned by the server
const bugContainer = ref(new BugPreviewContainer());

//Stores project information
const project = ref(new ProjectViewModel());

//If true, shows a loading animation on the bug list
const loading = ref(false);

//If true, shows the dialog for creating a filter
const showFilterDialog = ref(false);

//Severity option values for filter form
const severityValues = [
  {
    label: "Low",
    value: Severity.Low
  },
  {
    label: "Medium",
    value: Severity.Medium
  },
  {
    label: "High",
    value: Severity.High
  },
];

//Status option values for filter form
const statusValues = [
  {
    label: "Open",
    value: Status.Open
  },
  {
    label: "Closed",
    value: Status.Closed
  },
];

//Values for date search type dropdown
const dateSearchValues = ["Date created", "Last updated"];

//Values for sort type dropdown
const sortTypeValues = ["Summary", "Date created", "Last updated"];

//Values for sort order dropdown
const sortOrderValues = ["Ascending", "Descending"];

//Stores the current bug filter
const bugFilterDTO = ref(new BugFilterDTO());

//Stores values of the filter form
const filterFormValues = ref({
  summary: "",
  dateSearch: dateSearchValues[DateSearch.CreatedDate],
  dateFrom: null,
  dateTo: null,
  creatorName: "",
  severityValues: [Severity.Low, Severity.Medium, Severity.High],
  statusValues: [Status.Open, Status.Closed],
  sortType: sortTypeValues[SortType.Name],
  sortOrder: sortOrderValues[SortOrder.Ascending]
});

const route = useRoute();
const authStore = useAuthStore();

onBeforeMount(async() =>{

  //Assigns project ID to bug filter
  bugFilterDTO.value.projectID = route.params.projectId.toString();

  await getBugs();

  const projectResponse = await axios.get(`/projects/get/${route.params.projectId}`);
  project.value = projectResponse.data;

});

//Gets bugs from the server
async function getBugs(){
  loading.value = true;
  try{
    const response = await axios.post(`/bugs/getbugpreviews`, bugFilterDTO.value);
    bugContainer.value = Object.assign(response.data)
    //Sets the maximum number of pages visible at one point
    numberOfPages.value = Math.ceil(bugContainer.value.numberOfBugs / 20)
  }
  catch{

  }
  finally{
    loading.value = false;
  }
}

//Opens the filter dialog
function openDialog(){
  showFilterDialog.value = true;
}

async function onPageUpdate(){
  bugFilterDTO.value.pageNumber = bugContainer.value.currentPage;
  await getBugs();
}

//Clears the filter
function resetFilter(){
  filterFormValues.value ={
  summary: "",
  dateSearch: dateSearchValues[DateSearch.CreatedDate],
  dateFrom: null,
  dateTo: null,
  creatorName: "",
  severityValues: [Severity.Low, Severity.Medium, Severity.High],
  statusValues: [Status.Open, Status.Closed],
  sortType: sortTypeValues[SortType.Name],
  sortOrder: sortOrderValues[SortOrder.Ascending]
};
}

//Function to submit the form
async function onSubmit() {

  showFilterDialog.value = false;

  bugFilterDTO.value.summary = filterFormValues.value.summary;
  bugFilterDTO.value.dateSearch = dateSearchValues.indexOf(filterFormValues.value.dateSearch);
  bugFilterDTO.value.dateFrom = filterFormValues.value.dateFrom === "" ? null : filterFormValues.value.dateFrom;
  bugFilterDTO.value.dateEnd = filterFormValues.value.dateTo === "" ? null : filterFormValues.value.dateTo;
  bugFilterDTO.value.creatorName = filterFormValues.value.creatorName;
  bugFilterDTO.value.severityValues = filterFormValues.value.severityValues;
  bugFilterDTO.value.statusValues = filterFormValues.value.statusValues;

  bugFilterDTO.value.sortType = sortTypeValues.indexOf(filterFormValues.value.sortType);
  bugFilterDTO.value.sortOrder = sortOrderValues.indexOf(filterFormValues.value.sortOrder);

  //Resets page number to 1
  bugFilterDTO.value.pageNumber = 1;

  await getBugs();
}

//Assigns chip colour to bug severity and status fields
function getChipColour(value: string){

  switch(value){
    case "Low":
      return "blue";
    case "Medium":
      return "amber";
    case "High":
    case "Closed":
      return "red";
    case "Open":
      return "green";
  }
}

</script>
