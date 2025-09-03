<!--The app's landing page-->
<template>
  <h2>Main Page</h2>
  <br/>
  <div class="q-pa-md">
  <!--Welcome banner. Hidden if user is logged in-->
    <QBanner class="bg-purple-8" v-if="!authStore.isLoggedIn()">
      <h4>Welcome to VueBugTracker</h4>
      <p>Register or log in to access more features.</p>
      <template v-slot:action>
        <QBtn @click="showLoginDialog" label="Login"/>
        <QBtn to="register" label="Register"/>
      </template>
    </QBanner>
    <!--Shows the 5 most recently edited projects the user created.
    Hidden if user is not logged in-->
    <div v-if="authStore.isLoggedIn()">
      <h5>Your projects</h5>
      <br/>
      <QCard style="width: auto;">
        <QCardSection>
          <div>
            <div class="q-pa-md row items-start q-gutter-md" v-if="userProjectsStatusCode === 200 && recentUserProjects.length > 0">
              <ProjectPreview v-for="x in recentUserProjects" :-project="(x as ProjectPreviewViewModel)" v-bind:key="(x as ProjectPreviewViewModel).id"/>
            </div>
            <div class="q-pa-md row items-start q-gutter-md" v-else-if="userProjectsStatusCode === 200 && recentUserProjects.length == 0">
              <p>You have not created any projects. <a href="/createproject">Create project</a></p>
            </div>
            <QBanner v-else-if="userProjectsStatusCode === 500">Cannot fetch projects.</QBanner>
            <div class="q-pa-md row items-start q-gutter-md" v-else>
              <QCard v-for="x in 5" style="width: 320px;" v-bind:key="x">
              <QSkeleton square height="320px"/>
              <QCardActions align="right">
                <QSkeleton type="QBtn"/>
              </QCardActions>
            </QCard>
            </div>
          </div>
        </QCardSection>
      </QCard>
    </div>
<br/>
    <!--Shows the 5 most recently edited projects the user can see.
    Always visible regardless of whether the user is logged in-->
    <div>
      <h5>Recently updated projects</h5>
      <br/>
      <QCard style="width: auto;">
        <QCardSection>
          <div>
          <div class="q-pa-md row items-start q-gutter-md" v-if="projectsStatusCode === 200">
            <ProjectPreview v-for="x in recentProjects" :-project="x" v-bind:key="(x as ProjectPreviewViewModel).id"/>
          </div>

            <QBanner v-else-if="projectsStatusCode === 500">Cannot fetch projects.</QBanner>
            <div class="q-pa-md row items-start q-gutter-md" v-else>
              <QCard v-for="x in 5" style="width: 320px;" v-bind:key="x">
              <QSkeleton square height="320px"/>
              <QCardActions align="right">
                <QSkeleton type="QBtn"/>
              </QCardActions>
            </QCard>
            </div>
          </div>
        </QCardSection>
        <QCardActions align="right">
          <QBtn to="/browse" label="View more"/>
        </QCardActions>
      </QCard>
    </div>
  </div>
</template>
<script setup lang="ts">

import { Dialog } from 'quasar';
import LoginDialog from '@/dialogs/LoginDialog.vue';
import { useAuthStore } from '@/stores/AuthStore';
import router from '@/router/router';
import ProjectPreview from '@/components/ProjectPreview.vue';
import { onMounted, ref } from 'vue';
import axios, { AxiosError } from 'axios';
import ProjectPreviewViewModel from '@/viewmodels/ProjectPreviewViewModel';

const authStore = useAuthStore();

//Stores the user's most recently edited projects
const recentUserProjects = ref();

//Stores the most recently edited projects the user can view
const recentProjects = ref();

//Stores the status code returned when fetching projects
const projectsStatusCode = ref();

//Stores the status code returned when fetching user projects
const userProjectsStatusCode = ref();

onMounted(async () => {
  await getRecentProjects();
  if (authStore.isLoggedIn()) await getRecentUserProjects();
});

//gets previews of the 5 most recently edited projects
//from the backend the user can see
async function getRecentProjects(){
  try{
    const response = await axios.get("/projects/getrecentprojects");

    const projects :ProjectPreviewViewModel[] = [];

  for(let i = 0; i < response.data.length; i++){
    projects.push(Object.assign(new ProjectPreviewViewModel, response.data[i]));
  }

    recentProjects.value = response.data;
    projectsStatusCode.value = response.status;
  }
    catch (ex){
    const error = ex as AxiosError;
    projectsStatusCode.value = error.status;
    console.log(error);
  }
}

//gets previews of the user's 5 most recently edited projects
//from the backend
async function getRecentUserProjects(){
  try{
    const response = await axios.get("/projects/getrecentprojects?getUserProjects=true");

    const projects :ProjectPreviewViewModel[] = [];

    for(let i = 0; i < response.data.length; i++){
      projects.push(Object.assign(new ProjectPreviewViewModel, response.data[i]));
    }
    recentUserProjects.value = response.data;
    userProjectsStatusCode.value = response.status;
  }
  catch (ex){
    const error = ex as AxiosError;
    userProjectsStatusCode.value = error.status;
    console.log(error);
  }
}

//Function to open the login dialog
function showLoginDialog(){
  //Opens dialog
Dialog.create({
  component: LoginDialog,
  componentProps:{

  }
}).onOk(() => {
  console.log("Called OK");
  router.go(0);
}).onCancel(() =>{
  console.log("Called cancel");
}).onDismiss(() => {
  console.log("Called either OK or Cancel");
})
}
</script>
