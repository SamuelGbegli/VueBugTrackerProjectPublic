<!--Dummy page to test components-->
<template>
  <h2>Sandbox</h2>
  <p>This page is for testing components only and is not part of the main application.</p>

  <QBtn @click="openDialog" label="Open confirmation dialog"/>
  <QBtn @click="openDialogWithConfirmation" label="Open confirmation dialog with checkbox"/>
<CommentPreview/>
</template>
<script setup lang="ts">
import BugForm from '@/components/BugForm.vue';
import CommentPreview from '@/components/CommentPreview.vue';
import ProjectForm from '@/components/ProjectForm.vue';
import ConfirmationDialog from '@/dialogs/ConfirmationDialog.vue';
  import { useAuthStore } from '@/stores/AuthStore';
import ProjectPreviewViewModel from '@/viewmodels/ProjectPreviewViewModel';
import { Dialog } from 'quasar';
import { onMounted, ref } from 'vue';

  const authStore = useAuthStore();
  const isLoggedIn = ref();

  onMounted(async ()=> {


  });

  function openDialog(){
      Dialog.create({
        component: ConfirmationDialog,
        componentProps:{
          requiresConfirmation: false
        }
      }).onOk(() =>{
        console.log("Clicked on Yes");
      }).onCancel(() => {
        console.log("Clicked on No");
      })
    };

    function openDialogWithConfirmation(){
      Dialog.create({
        component: ConfirmationDialog,
        componentProps:{
          requiresConfirmation: true
        }
      }).onOk(() =>{
        console.log("Clicked on Yes");
      }).onCancel(() => {
        console.log("Clicked on No");
      })
    };

</script>
