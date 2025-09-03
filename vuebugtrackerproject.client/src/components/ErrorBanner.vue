<!--Asks the user to log in on protected resources-->
<template>
  <div>
    <QBanner class="bg-red-8">
      <p>{{ message? message :"You must be logged in to view this resource." }}</p>
      <template v-slot:action>
        <QBtn v-if="(props.promptType === ErrorPromptType.LoginButtonOnly || props.promptType === ErrorPromptType.LoginAndGoBackButtons)"
        @click="showLoginDialog"
        label="Login"/>
        <QBtn v-if="(props.promptType === ErrorPromptType.GoBackButtonOnly || props.promptType === ErrorPromptType.LoginAndGoBackButtons)"
        @click="router.back"
        label="Go back"/>
        <QBtn v-if="(props.promptType === ErrorPromptType.ReloadButton)"
        @click="router.go(0)"
        label="Reload"/>
      </template>
    </QBanner>
  </div>
</template>
<script setup lang="ts">

import { Dialog } from 'quasar';
import LoginDialog from '@/dialogs/LoginDialog.vue';
import router from '@/router/router';
import ErrorPromptType from '@/enumConsts/ErrorPromptType';

const props = defineProps({
  message: String,
  promptType: Number
});

//Function to open the login dialog
function showLoginDialog(){
  //Opens dialog
Dialog.create({
  component: LoginDialog,
  componentProps:{

  }
}).onOk(() => {
  router.go(0);
}).onCancel(() =>{
}).onDismiss(() => {
})
}
</script>
