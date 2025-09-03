<!--Page for modifying a user's settings-->
<template>
  <div v-if="authStore.isLoggedIn()">
    <h5>Account settings</h5>
    <div class="q-pa-md q-gutter-md row">
      <div class="col-2">
        <QTabs
          v-model="currentTab"
          vertical>
            <QTab name="Avatar" label="Avatar"/>
            <QTab name="Credentials" label="Credentials"/>
            <QTab name="Other" label="Other"  v-if="authStore.user.role != AccountRole.SuperUser"/>
          </QTabs>
      </div>
      <div class="col">
        <QTabPanels
          v-model="currentTab"
          vertical>
            <QTabPanel name="Avatar">
              <!--Change avatar section-->
              <div class="q-gutter-md">
                <p>This part of the site is not functional, instead being a demonstration for how updating a user profile could look like.</p>
                <ul>
                  <li>Images must be a .png, .jpeg or .gif file, and must be 1 MB or less.</li>
                  <li>Anything you upload may be downsized.</li>
                </ul>
                <div class="row"><!--
                  <div class="col">
                    <h5>Current avatar</h5>
                    Show larger version of current user avatar
                  </div>-->
                  <div class="col">
                    <QUploader
                        auto-upload
                        label="Upload icon"
                        accept="image/jpeg, image/png, image/gif"
                        style="max-width: 300px;" />

                  </div>
                </div>
              </div>
            </QTabPanel>
            <QTabPanel name="Credentials">
              <div class="q-gutter-md">
              <!--Change email address section-->
                <QCard>
                  <QForm @submit="onSubmit(AccountChangeValue.Email)" style="max-width: 500px;">
                    <QCardSection>
                      <p>Use the form to change your email address.</p>
                        <QInput label="Email address"
                          stack-label
                          v-model="email"
                          :rules="[val => isFieldEmpty(val), val => isEmailValid(val)]"/>
                    </QCardSection>
                    <QCardActions align="right">
                      <QBtn type="submit" label="Submit"/>
                    </QCardActions>
                  </QForm>
                </QCard>
                <!--Change username section-->
                <QCard>
                  <QForm @submit="onSubmit(AccountChangeValue.Username)" style="max-width: 500px;">
                    <QCardSection>
                      <p>Use the form to change your username. Usernames must be at least 4 characters long and not be used by an existing user.</p>
                      <QInput label="Username"
                        stack-label
                        v-model="username"
                        :rules="[val => isFieldEmpty(val), val => isUsernameValid(val)]"/>
                    </QCardSection>
                    <QCardActions align="right">
                      <QBtn type="submit" label="Submit"/>
                    </QCardActions>
                  </QForm>
                </QCard>
                <!--Change password section-->
                <QCard>
                  <QForm @submit="onSubmit(AccountChangeValue.Password)" style="max-width: 500px;">
                    <QCardSection>
                      <p>Use the form to change your password. Passwords must have at least:</p>
                        <ul>
                          <li>8 characters</li>
                          <li>an upper case character</li>
                          <li>a lower case character</li>
                          <li>a digit</li>
                        </ul>
                        <QInput label="Existing password"
                          stack-label
                          type="password"
                          v-model="existingPassword"
                          :rules="[val => isFieldEmpty(val)]"/>
                        <QInput label="New password"
                          stack-label
                          type="password"
                          v-model="newPassword"
                          :rules="[val => isFieldEmpty(val), val => isPasswordValid(val)]"/>
                        <QInput label="Confirm new password"
                          stack-label
                          type="password"
                          v-model="passwordConfirm"
                          :rules="[val => isFieldEmpty(val), val => isPasswordConfirmValid()]"/>
                    </QCardSection>
                    <QCardActions align="right">
                      <QBtn type="submit" label="Submit"/>
                    </QCardActions>
                  </QForm>
                </QCard>
              </div>
            </QTabPanel>
            <QTabPanel name="Other" v-if="authStore.user.role != AccountRole.SuperUser">
              <QCard>
                <QCardSection>
                  <div class="row">
                    <div class="col">
                      Click the button on the right to delete your account. This is an irreversable process, and you will be asked to confirm your choice.
                    </div>
                    <div class="col-2">
                      <QBtn @click="deleteAccount" label="Delete account"/>
                    </div>
                  </div>
                </QCardSection>
              </QCard>
            </QTabPanel>
          </QTabPanels>
      </div>
    </div>
  </div>
  <ErrorBanner v-else
    message="You must be logged in to view this resource."
    :prompt-type="ErrorPromptType.LoginAndGoBackButtons"/>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { useAuthStore } from '@/stores/AuthStore';
import ErrorBanner from '@/components/ErrorBanner.vue';
import ErrorPromptType from '@/enumConsts/ErrorPromptType';
import { Dialog, Loading, Notify } from 'quasar';
import ConfirmationDialog from '@/dialogs/ConfirmationDialog.vue';
import axios, { AxiosError } from 'axios';
import { useRouter } from 'vue-router';
import AccountChangeValue from '@/enumConsts/AccountChangeValue';
import PasswordDTO from '@/classes/DTOs/PasswordDTO';
import AccountRole from '@/enumConsts/Role';

//Used for changing the user's email
const email = ref("");
//Used for changing the user's username
const username = ref("");
//Used to store the user's existing password
const existingPassword = ref("");
//Used for changing the user's new password
const newPassword = ref("");
//Used to verify the user's password change
const passwordConfirm = ref("");

const authStore = useAuthStore();
const router = useRouter();

//Stores the tab the user is currently on
const currentTab = ref("Avatar");

//Checks if a field is empty
function isFieldEmpty(input: string){
  if(!!input) return true;
  return "Field is required.";
}


//Checks if an email is valid
function isEmailValid(input: string){

  if(/(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])/.test(input))
  return true;

  return "Email is invalid.";
}

//Checks if a username is valid
async function isUsernameValid(input: string){
  if(input.length < 4) return "Usernames must be at least 4 characters long.";

  //Checks if username is taken by another user

  try{
    const response = await axios.get(`accounts/verifyusername/${input}`)
    if(response.data) return "Username has been taken."
  }
  catch {
    return "Username cannot be validated. Please try again later.";
  }

  return true;
}

//Checks if the user's new password is valid
function isPasswordValid(input: string){
  const errors: string[] = [];
  if(input.length < 8) errors.push("Password must be at least 8 characters long.");
  if(!/[A-Z]/.test(input)) errors.push("At least 1 upper case letter is required.");
  if(!/[a-z]/.test(input)) errors.push("At least 1 lower case letter is required.");
  if(!/[0-9]/.test(input)) errors.push("At least 1 digit letter is required.");

  if(errors.length === 0) return true;

  let message: string = "";
  for (let i = 0; i < errors.length; i++){
    message += `${errors[i]}\n`;
  }
  return message;
}

//Checks if password confirmation is valid
function isPasswordConfirmValid(){
  return (newPassword.value === passwordConfirm.value || "Passwords do not match.");
}

//Function to save changes to the user's credentials
async function onSubmit(credentials:number) {
  try {
    Loading.show({
      message: "Please wait..."
    });
    switch (credentials){
      case AccountChangeValue.Email:
        await axios.patch("/accounts/updateemail", email.value,{
          headers: {"Content-Type": "application/json"}});
        email.value = "";
        break;
      case AccountChangeValue.Username:
        await axios.patch("/accounts/updateusername", username.value,{
          headers: {"Content-Type": "application/json"}});

          //Ensures user name changes on screen
          authStore.$patch((state) => {
            state.user.username = username.value;
          })
        username.value = "";
        break;
      case AccountChangeValue.Password:
        const passwordDTO = new PasswordDTO();
        passwordDTO.oldPassword = existingPassword.value;
        passwordDTO.newPassword = newPassword.value;

        await axios.patch("/accounts/updatepassword", passwordDTO);

        existingPassword.value = "";
        newPassword.value = "";
        passwordConfirm.value = "";
        break;
    }
    Notify.create({
        message: "Successfully saved changes.",
        position: "bottom",
        type: "positive"
      });
  } catch (ex){
    const error = ex as AxiosError;
    Notify.create({
        message: error.status === 401 ? "The password you inputted was incorrect. Please try again." :
        "Something went wrong when processing your request. Please try again later.",
        position: "bottom",
        type: "negative"
      });
  }
  Loading.hide();
}

//Deletes the user's account
async function deleteAccount(){
  Dialog.create({
    component: ConfirmationDialog,
    componentProps:{
      requiresConfirmation: true,
      header: "Delete account",
      message: "Deleting your account is an irreversable process. If you continue, all projects you made will also be deleted.\n Are you sure you want to continue?",
    }
  }).onOk(async ()=> {
    Loading.show({
      message: "Please wait..."
    });
    try {
      await axios.post("/auth/logout", {});
      await axios.delete("/accounts/deleteaccount",{
        headers: {"Content-Type": "application/json"},
        data: authStore.getUserID()
      });
      authStore.logout();
      router.push({path: "/"});
    } catch {
      Notify.create({
        message: "Something went wrong when processing your request. Please try again later.",
        position: "bottom",
        type: "negative"
      });
    }
    Loading.hide();
  });
}

</script>
