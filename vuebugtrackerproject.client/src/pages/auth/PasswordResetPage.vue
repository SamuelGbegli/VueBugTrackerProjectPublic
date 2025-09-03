<template>
<div v-if="isTokenValid">
  <h4>Password reset</h4>
<br/>
<QBanner class="bg-primary">
  Use the form below to change your account's password. Passwords must have at least:
  <ul>
    <li>8 characters</li>
    <li>an upper case character</li>
    <li>a lower case character</li>
    <li>a digit</li>
  </ul>
</QBanner>
<br/>
<QCard style="width: 600px;">
  <QForm @submit="onSubmit" style="padding: 10px;">
    <QCardSection>
  <QInput label="Password"
          stack-label
          type="password"
          v-model="password"
          :rules="[val => isFieldEmpty(val), val => isPasswordValid(val)]"/>
  <QInput label="Confirm password"
          stack-label
          type="password"
          v-model="passwordConfirm"
          :rules="[val => isFieldEmpty(val), val => isPasswordConfirmValid()]"/>
    </QCardSection>
    <QCardActions align="right">
    <QSpace/>
      <QBtn type="submit" label="Reset password"/>
    </QCardActions>
</QForm>
</QCard>
</div>
<QBanner v-else class="bg-red">
  Either the reset token you supplied is invalid or the server is not responding.
</QBanner>
</template>
<script setup lang="ts">

import { onMounted, ref } from 'vue';
import PasswordResetDTO from '@/classes/DTOs/PasswordResetDTO';
import axios, { AxiosError } from 'axios';
import { useAuthStore } from '@/stores/AuthStore';
import router from '@/router/router';
import { useRoute } from 'vue-router';
import { Loading, Notify } from 'quasar';
import UserDTO from '@/classes/DTOs/UserDTO';

const authStore = useAuthStore();
const route = useRoute();

const isTokenValid = ref(false);
const password = ref("");
const passwordConfirm = ref("");

async function validateToken(){
    try{
  const response = await axios.get(`/auth/validateresettoken?id=${route.query.id}&token=${route.query.token}`);

  isTokenValid.value = response.data;
}
catch(ex){
const error = ex as AxiosError;
console.log(error);
}
  }

onMounted(() =>{
  validateToken();
})



//Checks if a field is empty
function isFieldEmpty(input: string){
  if(!!input) return true;
  return "Field is required.";
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

//Creates DTO to reset password
const passwordResetDTO = new PasswordResetDTO();
passwordResetDTO.accountID = route.query.id as string;
passwordResetDTO.password = password.value;
passwordResetDTO.passwordResetToken = route.query.token as string;

try{
  //Sends response to user
  const response = await axios.post("/auth/resetpassword", passwordResetDTO);

  //Creates user DTO with username returned from backend
  const userDTO = new UserDTO();
  userDTO.Username = response.data;
  userDTO.Password = password.value;

  //Automatically logs in user
  const loginResponse = await authStore.login(userDTO);

//Redirects to main page when login is successful
if(loginResponse === 200) router.replace({path:"/"});
//Shows error message if password is changed and and the server fails to log in
else{
  Notify.create({
message: "Something went wrong when logging in. Youe account's password was successfully changed, so please log in with your credentials later.",
position: "bottom",
type: "negative"
});
}
}
catch {
  //Server could not change password, error message is shown
  Notify.create({
message: "Could not save changes. Please try again later.",
position: "bottom",
type: "negative"
});
}

Loading.hide();
}

</script>
