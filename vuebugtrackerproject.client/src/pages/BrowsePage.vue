<!--Page for browsing projects-->
<template>
  <div class="q-gutter-md">
    <div class="row">
    <h3>Browse</h3>
    <QSpace/>
  </div>
  <div v-if="!statusCode || statusCode === 200">
    <div class="row q-gutter-md">
      <ProjectPreview v-for="x in projects.projects" :-project="x" v-bind:key="(x as ProjectPreviewViewModel).id"/>
    </div>
      <div class="row">
        <QSpace/>
        <QPagination v-model="currentPage"
                   v-if="!!statusCode"
                   :min="1"
                   :max="numberOfPages"
                   @update:model-value="router.push({path: `/browse/${currentPage}`})"
                   input />
      </div>
      <QInnerLoading style="height: 100%;"
        :showing="!statusCode"/>
    </div>
    <div v-else>
      <ErrorBanner :prompt-type="statusCode === 500? ErrorPromptType.ReloadButton : ErrorPromptType.GoBackButtonOnly"
      :message="statusCode === 500? 'There was an error connecting with the server.' : 'The page you requested does not exist.'"/>
    </div>
  </div>
</template>
<script setup lang="ts">
import ErrorBanner from '@/components/ErrorBanner.vue';
import ProjectPreview from '@/components/ProjectPreview.vue';
import ErrorPromptType from '@/enumConsts/ErrorPromptType';
import ProjectContainer from '@/viewmodels/ProjectContainer'
import type ProjectPreviewViewModel from '@/viewmodels/ProjectPreviewViewModel';
import axios, { AxiosError } from 'axios';
import { onBeforeMount, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';

//Projects visible by the user
const projects = ref(new ProjectContainer());

//The page the user is on
const currentPage = ref(1);

//The number of pages the pagination control will see
const numberOfPages = ref(1);

//The status code of the request to get projects
const statusCode = ref();

const route = useRoute();
const router = useRouter();

onBeforeMount(async () =>{

  try{
    //Sets the value of the pagination control

    currentPage.value = !!route.params.page ? parseInt(route.params.page.toString()) : 1;

    //Gets projects from server
    const response = await axios.get(`/projects/get?pageNumber=${!!currentPage.value? currentPage.value: 1}`);

    //Assigns project view model objects
    projects.value = Object.assign(new ProjectContainer(), response.data);

    //Sets the maximum value of the pagination control
    numberOfPages.value = Math.ceil(projects.value.totalProjects / 30);

    //Sets status code
    //If no projects are found, sets a 404 error message
    if(projects.value.projects.length === 0) statusCode.value = 404;
    //Otherwise sets 200 from the server
    else statusCode.value = response.status;

    console.log(response.status);
  }
  catch(ex){
    //Sets status code error
    const error = ex as AxiosError;
    statusCode.value = error.status;
  }
})

</script>
