<template>
  <div class="q-pa-md row q-gutter-md">
    <div class="col-8">
      <QCard style="min-height: 60vh">
        <QCardSection v-if="statusCode === 200">
          <h5>Description</h5>
          <div v-html="bug.description"/>
        </QCardSection>
        <QCardSection v-else>
          <QSkeleton square height="500px"/>
        </QCardSection>
      </QCard>
    </div>
    <div class="col">
      <QCard>
        <div v-if="statusCode === 200">
          <QCardSection>
          <h5>Bug details</h5>
        </QCardSection>
        <QCardSection>
          <h6>Date created</h6>
          <span>{{ formatDate(bug.dateCreated) }}</span>
        </QCardSection>
        <QCardSection>
          <h6>Created by</h6>
          <UserIcon :username="bug.creatorName" :icon="bug.creatorIcon"/>
        </QCardSection>
        <QCardSection>
          <h6>Status</h6>
          <QChip :color="getChipColour(bug.isOpen ? 'Open' : 'Closed' )">
            {{ bug.isOpen ? "Open" : "Closed" }}
          </QChip>
        </QCardSection>
        <QCardSection>
          <h6>Severity</h6>
          <QChip :color="getChipColour(bugSeverity[bug.severity])">
            {{ bugSeverity[bug.severity] }}
          </QChip>
        </QCardSection>
        <QCardSection v-if="editStatusCode === 200">
          <QBtn @click="showBugToggleDialog" :label="bug.isOpen ? 'Close bug' : 'Reopen bug'"/>
        </QCardSection>
        </div>
        <div v-else>
          <QCardSection>
            <QSkeleton square height="500px"/>
          </QCardSection>
        </div>
      </QCard>
    </div>
  </div>
  </template>
  <script setup lang="ts">

  import axios, { AxiosError } from 'axios';
  import { onBeforeMount, ref } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import UserIcon from '@/components/UserIcon.vue';
import BugViewModel from '@/viewmodels/BugViewModel';
import { Dialog, Loading, Notify } from 'quasar';
import ConfirmationDialog from '@/dialogs/ConfirmationDialog.vue';
import formatDate from '@/classes/helpers/FormatDate';
import getChipColour from '@/classes/helpers/GetChipColour';

const bugSeverity = ["Low", "Medium", "High"];

    const bug = ref(new BugViewModel());
    const statusCode = ref();
    const editStatusCode = ref();
    const route = useRoute();
    const router = useRouter();

    onBeforeMount(async ()=>{
      //TODO: add error handling for when loading project fails
      try{
        const response = await axios.get(`/bugs/get/${route.params.bugId}`);
        statusCode.value = response.status;
        bug.value = response.data;
        await checkIfUserCanEdit()
      }
      catch (ex){
        const error = ex as AxiosError;
        statusCode.value = error.status;
      }
    });

//Shows dialog to open or close a bug
  function showBugToggleDialog(){
    Dialog.create({
      component: ConfirmationDialog,
      componentProps:{
        header: bug.value.isOpen ? "Close bug" : "Reopen bug",
        message: bug.value.isOpen ? "Are you sure you want to close this bug?"
        : "Are you sure you want to reopen this bug?"
      }
    }).onOk(async () =>{
      await toggleBug();
    })
  }

//Opens a closed bug, and vice versa
    async function toggleBug(){
      Loading.show({
        message: "Please wait..."
      });
      try{
        await axios.patch("/bugs/togglebugstatus", bug.value.id,{
          headers: {"Content-Type": "application/json"}
        });
        router.go(0);
      }
      //Section if something goes wrong
      catch{
        Notify.create({
          message: "Something went wrong when processing your request. Please try again later.",
          position: "bottom",
          type: "negative"
        });
      }
      Loading.hide();
    }

//Function to verify if the user can edit the project's bug
async function checkIfUserCanEdit(){
  try{
    const response = await axios.get(`/projects/canedit/${bug.value.projectID}`);
    editStatusCode.value = response.status;
  }
  catch (ex) {
    const error = ex as AxiosError;
    editStatusCode.value = error.status;
  }
}
  </script>
