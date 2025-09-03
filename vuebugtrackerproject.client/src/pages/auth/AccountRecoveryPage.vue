<!--Page for a user to help regain access to their account
if they forget their password-->

<template>
  <h4>Account recovery</h4>
<br/>
<QBanner class="bg-primary">
  If you forgot the password to your account, you can use the form below to reset it.
</QBanner>
<br/>
<QCard style="width: 600px;" v-if="!isFormSubmitted">
  <QForm @submit="onSubmit" style="padding: 10px;">
    <QInput label="Username"
          stack-label
         v-model="username"
          :rules="[val => isFieldEmpty(val)]">
          </QInput>
      <QInput label="Email address"
          stack-label
          v-model="email"
          :rules="[val => isFieldEmpty(val), val => isEmailValid(val)]">
          </QInput>
        <QCardActions align="right">
          <QBtn type="submit" label="Submit" />
        </QCardActions>
      </QForm>
</QCard>
<QBanner style="background-color: green" v-else>
  Form has been successfuly submitted. Please check your email inbox for further instructions.
</QBanner>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import UserDTO from '@/classes/DTOs/UserDTO';
import axios, { AxiosError } from 'axios';
import { Loading, Notify } from 'quasar';

//The account's email address
const email = ref("");

//The account's username
const username = ref("");

//Hides the form if true
const isFormSubmitted = ref(false);

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

async function onSubmit(){

  //Shows loading display
  Loading.show({
  message: "Please wait..."
});

  //Creates DTO to send username and email address
  const userDTO = new UserDTO();
  userDTO.Username = username.value;
  userDTO.Emailaddress = email.value;

  try{
    //Sends a post request to the server
    await axios.post("/auth/passwordresetrequest", userDTO);

    //Hides the form if the request was successful
    isFormSubmitted.value = true;
  }
  //Shows an error message when the server cannot process the request
  catch (ex){

    const error = ex as AxiosError;

    if(error.status === 500){
      Notify.create({
    message: "Could not process request. Please try again later.",
    position: "bottom",
    type: "negative"
  });
    }
    else{
         //Shows an error message if an account cannot be found with the username and email given
         Notify.create({
    message: "Could not find an account with the credentials given. Please review them and try again.",
    position: "bottom",
    type: "negative"
  });
    }


  }

  Loading.hide();
}

</script>
