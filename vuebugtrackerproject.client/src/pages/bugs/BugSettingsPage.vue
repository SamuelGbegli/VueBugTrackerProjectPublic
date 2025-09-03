<template>
  <div>
  <div class="q-pa-md q-gutter-md row">
  <div class="col-1">
  <QTabs
  v-model="currentTab"
  vertical>
    <QTab name="Edit bug" label="Edit bug"/>
    <QTab name="Other" label="Other"/>
  </QTabs>
  </div>
<div class="col">
<QTabPanels v-model="currentTab"
      vertical>
    <QTabPanel name="Edit bug">
      <div>
        <QBanner>
    Use the form below to edit the bug.
  </QBanner>
  <br/>
    <div>
      <BugForm v-if="!!bug" :-bug="bug"/>
      <QInnerLoading
      :showing="!statusCode"/>
    </div>
      </div>
    </QTabPanel>
    <QTabPanel name="Other">
      <QCard>
        <QCardSection>
          <div class="row">
        <div class="col">
          Click the button on the right to {{bug.isOpen? "close" : "reopen"}} the bug.
        </div>
        <div class="col-2">
          <QBtn @click="showBugToggleDialog" :label="bug.isOpen? 'Close bug' : 'Reopen bug'"/>
        </div>
      </div>
        </QCardSection>
      </QCard>
      <br/>
      <QCard>
        <QCardSection>
          <div class="row">
            <div class="col">
              Click the button on the right to delete the bug.
            </div>
            <div class="col-2">
              <QBtn @click="deleteBug" label="Delete bug"/>
            </div>
          </div>
        </QCardSection>
      </QCard>
    </QTabPanel>
    </QTabPanels>
</div>

  </div>
  </div>
</template>
<script setup lang="ts">
  import axios, { AxiosError } from 'axios';
  import { onBeforeMount, ref } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
import { Dialog, Loading, Notify } from 'quasar';
import BugForm from '@/components/BugForm.vue';
import ConfirmationDialog from '@/dialogs/ConfirmationDialog.vue';
import BugDTO from '@/classes/DTOs/BugDTO';

    const bug = ref();
    const statusCode = ref();
    const route = useRoute();
    const router = useRouter();
    //const showDialog = ref(false);
    const currentTab = ref("Edit bug");

    onBeforeMount(async ()=>{
      getBug()
    });

    //Gets bug from the backend
    async function getBug(){
      try{
        const response = await axios.get(`/bugs/get/${route.params.bugId}`);
        statusCode.value = response.status;
        bug.value = response.data;
        console.log(bug.value);
      }
      catch (ex){
        const error = ex as AxiosError;
        statusCode.value = error.status;
      }
    }

    //Shows dialog to open a closed bug, and vice versa
    function showBugToggleDialog(){
    Dialog.create({
      component: ConfirmationDialog,
      componentProps:{
        header: bug.value.isOpen ? "Close bug" : "Reopen bug",
        message: bug.value.isOpen ? "Are you sure you want to close this bug?"
        : "Are you sure you want to reopen this bug?"
      }
    }).onOk(async () =>{

      Loading.show({
        message: "Please wait..."
      });
      try{
        await axios.patch("/bugs/togglebugstatus", bug.value.id,{
          headers: {"Content-Type": "application/json"}
        });
        getBug()
      }
      catch{
        Notify.create({
          message: "Something went wrong when processing your request. Please try again later.",
          position: "bottom",
          type: "negative"
        });
      }
      Loading.hide();
    })
  }

  // Deletes bug from project
  async function deleteBug(){
    //Creates delete dialog
    Dialog.create({
      component: ConfirmationDialog,
      componentProps:{
        confirmationRequired: true,
        header: "Delete bug",
        message: "Deleting a bug is an irreversable process. Are you sure you want to continue?"
      }
    }).onOk( async () =>{
      const bugDTO = new BugDTO();
      bugDTO.bugID = bug.value.id;
      bugDTO.projectID = bug.value.projectID;
      try{
        await axios.delete("/bugs/deletebug", {
        headers:{

        },
        data: bugDTO
      });
      router.push(`/project/${bugDTO.projectID}/bugs`);
      }
      catch{
        Notify.create({
          message: "Could not process request. Please try again later.",
          position: "bottom",
          type: "negative"
        });
      }
    });
  }
</script>
