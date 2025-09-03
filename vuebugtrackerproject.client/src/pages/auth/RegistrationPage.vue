<!--Page for creating an account-->
<template>
<h4>Register</h4>
<br/>
<QBanner class="bg-primary">
  Use the form below to create an account.
</QBanner>
<br/>
<QCard style="width: 600px;">
  <QForm @submit="onSubmit" style="padding: 10px;">
  <QCardSection>
      <QInput label="Email address"
          stack-label
          v-model="email"
          :rules="[val => isFieldEmpty(val), val => isEmailValid(val)]">
          <template v-slot:append>
            <QIcon name="help">
              <QTooltip anchor="center right" self="center left">
                This will be used to recover your account if you forget your password.
              </QTooltip>
            </QIcon>
          </template>
          </QInput>
  <QInput label="Username"
          stack-label
         v-model="username"
          :rules="[val => isFieldEmpty(val), val => isUsernameValid(val)]">
          <template v-slot:append>
            <QIcon name="help">
              <QTooltip anchor="center right" self="center left">
                Usernames must be at least 4 characters long and not be used by another user.
              </QTooltip>
            </QIcon>
          </template>
          </QInput>
  <QInput label="Password"
          stack-label
          type="password"
          v-model="password"
          :rules="[val => isFieldEmpty(val), val => isPasswordValid(val)]">
            <template v-slot:append>
              <QIcon name="help">
              <QTooltip anchor="center right" self="center left">
                Passwords must be at least 8 characters long and have at least one:
                <ul>
                  <li>upper case letter</li>
                  <li>lower case letter</li>
                  <li>digit</li>
                </ul>
              </QTooltip>
            </QIcon>
            </template>
            </QInput>
  <QInput label="Confirm password"
          stack-label
          type="password"
          v-model="passwordConfirm"
          :rules="[val => isFieldEmpty(val), val => isPasswordConfirmValid()]"/>
  </QCardSection>
    <QCardActions align="right">
    <QSpace/>
      <QBtn type="submit" label="Create account"/>
    </QCardActions>
</QForm>
</QCard>
</template>
<script setup lang="ts">

import { ref } from 'vue';
import UserDTO from '@/classes/DTOs/UserDTO';
import axios from 'axios';
import { useAuthStore } from '@/stores/AuthStore';
import router from '@/router/router';
import { Loading, Notify, QIcon, QSpace, QTooltip } from 'quasar';

const email = ref("");
const username = ref("");
const password = ref("");
const passwordConfirm = ref("");

const authStore = useAuthStore();

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

//Checks if a password is valid
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
  return (password.value === passwordConfirm.value || "Passwords do not match.");
}

//Submits the form
async function onSubmit(){

//Sets loading display
Loading.show({
  message: "Please wait..."
});


const userDTO = new UserDTO();
userDTO.Emailaddress = email.value;
userDTO.Password = password.value;
userDTO.Username = username.value;

try{
  //Sends user info to server
  const response = await axios.post("auth/createaccount", userDTO);

  //Account has been created
  if(response.status === 204){

    //Automatically logs in new account
    const loginResponse = await authStore.login(userDTO);

    //Refreshes page if login is successful
    if(loginResponse === 200) router.go(0);
    //Shows error message if account is created and the server fails to log in
    else{
      Notify.create({
    message: "Something went wrong when logging in. Your account has been created, so please log in with your credentials later.",
    position: "bottom",
    type: "negative"
  });
    }
  }
}
catch {
  //Something went wrong, and show error notification
  Notify.create({
    message: "Cannot connect to server. Please try again later.",
    position: "bottom",
    type: "negative"
  });
}

//Hides loading display
Loading.hide();

}


</script>
