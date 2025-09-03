<template>
    <div v-if="statusCode === 200">
  <div class="q-gutter-md">
    <QBanner>
      <h3>{{ project.name }}</h3>
      <h6>{{ project.summary }}</h6>
      <h6>Last updated: {{ formatDate(project.dateModified )}}</h6>
    </QBanner>
    <QTabs align="left">
      <QRouteTab label="Main" :to="`/project/${project.id}`"/>
      <QRouteTab label="Bugs" :to="`/project/${project.id}/bugs`"/>
      <!--hidden if user does not own project-->
      <QRouteTab v-if="!!authStore.user && authStore.user.id === project.ownerID" label="Settings" :to="`/project/${project.id}/settings`"/>
    </QTabs>
    <router-view />
  </div>
  <div v-if="(statusCode && statusCode != 200)">
    <ErrorBanner :message="errorMessage" :prompt-type="errorPromptType"/>
  </div>
  </div>
</template>
<script setup lang="ts">
import axios, { AxiosError } from 'axios';
import { onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';
import { useAuthStore } from '@/stores/AuthStore';
import ProjectViewModel from '@/viewmodels/ProjectViewModel';
import ErrorBanner from '@/components/ErrorBanner.vue';
import ErrorPromptType from '@/enumConsts/ErrorPromptType';
import formatDate from '@/classes/helpers/FormatDate';

const authStore = useAuthStore();

const project = ref(new ProjectViewModel());
const statusCode = ref();
const errorMessage = ref("");
const errorPromptType = ref();

const route = useRoute();

onMounted(async ()=>{

  try{
  //Loads project from database
    const response = await axios.get(`/projects/get/${route.params.projectId}`);
    //Converts JSON to Project view model
    project.value = Object.assign(new ProjectViewModel(), response.data);
    //Updates status code to show the project
    statusCode.value = response.status;

  }
  catch(ex){
    //Error handling if project could not be fetched
    const error = ex as AxiosError;
    //Updates status code to show error banner
    statusCode.value = error.status;
    //Chooses error message when a project isn't returned
    switch(error.status){
      //User that isn't logged in accesses project only visible to logged in users
      case 401:
        //Restricted projects
        if(error.message === "Restricted project")
          errorMessage.value = "The project's owner has restricted access. Please log in if you've been granted access.";
        //
          else
          errorMessage.value = "You must be logged in to view this page.";
        errorPromptType.value = ErrorPromptType.LoginAndGoBackButtons;
        break;
      //User attempts to view restricted project they don't have permission to view
      case 403:
        errorMessage.value = "You do not have permission to view this page.";
        errorPromptType.value = ErrorPromptType.GoBackButtonOnly;
        break;
      //Project does not exist
      case 404:
        errorMessage.value = "The page you requested does not exist.";
        errorPromptType.value = ErrorPromptType.GoBackButtonOnly;
        break;
      //Assumes server could not return a valid response
      default:
        errorMessage.value = "There was an error connecting to the server.";
        errorPromptType.value = ErrorPromptType.ReloadButton;
        break;
    }
  }
})
</script>
