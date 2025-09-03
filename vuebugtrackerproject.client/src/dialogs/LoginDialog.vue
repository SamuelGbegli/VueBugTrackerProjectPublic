<!--Dialog for logging into the app-->
<template>
  <QDialog ref="dialogRef"
  persistent
   @hide="onDialogHide">
    <QCard class="q-dialog-plugin" style="margin: 4px;">
      <QCardSection class="row items-center">
        <h5>Login</h5>
        <QSpace />
        <QBtn icon="close" flat v-close-popup />
      </QCardSection>
      <QForm @submit="onSubmit">
        <QCardSection class="items-center" style="margin: 10px">
          <!--Form goes here-->
          <QInput label="Username"
          stack-label
          v-model="username"
          :rules="[val => !! val || 'Please fill out this field.']" />
          <QInput type="password"
          label="Password"
          stack-label
          v-model="password"
          :rules="[val => !! val || 'Please fill out this field.']" />
          <QBanner v-if="error" inline-actions class="text-white bg-red">
            {{ error }}
            <template v-slot:action>
              <QBtn icon="close" flat @click="() => error = ''" />
            </template>
          </QBanner>
        </QCardSection>
        <QCardActions align="right">
          <QBtn @click="onCloseClick" label="Close" />
          <QBtn type="submit" label="Login" />
        </QCardActions>
      </QForm>
      <QCardSection>
        <RouterLink to="/recoveraccount">
          <span @click="onDialogCancel()">Forgot your password?</span>
        </RouterLink>
      </QCardSection>
    </QCard>
  </QDialog>
</template>
<script setup lang="ts">

  import { ref } from 'vue';
  import { useDialogPluginComponent, Loading } from 'quasar';

  import UserDTO from '../classes/DTOs/UserDTO';
import { useAuthStore } from '@/stores/AuthStore';

  const username = ref("");
  const password = ref("");

  const error = ref("");

  const authStore = useAuthStore();

  defineEmits([
    ...useDialogPluginComponent.emits
  ]);

  const { dialogRef, onDialogHide, onDialogOK, onDialogCancel } = useDialogPluginComponent();

  function onCloseClick() {
    onDialogCancel();
  }

  //Function to submit the form
  async function onSubmit() {

    //Shows loading screen
    Loading.show({
      message: "Please wait..."
    });

    //Creates DTO to send to the server
    const userDTO = new UserDTO();

    userDTO.Username = username.value;
    userDTO.Password = password.value;

    //Logs the user in
    const response = await authStore.login(userDTO);

    //If login is successful, close dialog
    if (response === 200) {
      console.log("Successfully logged in");
      onDialogOK();
    }
    //Shows error message if login failed
    else
      console.log(`Failed to login. Error message: ${response}`)
    error.value = response;

      //Hides loading screen
      Loading.hide();
  }

</script>
