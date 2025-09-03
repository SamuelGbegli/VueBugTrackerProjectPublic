<!--Form to add or edit a bug-->
<template>
<QCard style=" max-width: 1200px;">
  <QForm @submit="onSubmit">
      <QCardSection>
        <div class="q-pa-md example-row-equal-width">
      <div class="row q-gutter-lg">
        <div class="col" style="max-width: 400px;">
          <QInput label="Summary"
                  stack-label
                  v-model="bugSummary"
                  :rules="[val => !! val || 'Please fill out this field.']" />
        </div>
        <div class="col" style="max-width: 400px;">
          <QSelect v-model="bugSeverity" label="Severity" stack-label :options="options"/>
        </div>
      </div>
      <br/>
      <QInput label="Description"
      stack-label
      v-model="bugDescription"
      type="textarea"
      style="max-width: 70%;"
      :rules="[val => !! val || 'Please fill out this field.']"/>

      </div>
      </QCardSection>
    <QCardActions>
      <QBtn type="submit" label="Submit"/>
    </QCardActions>
    </QForm>
</QCard>
</template>
<script setup lang="ts">
import BugDTO from '@/classes/DTOs/BugDTO';
import Severity from '@/enumConsts/Severity';
import { onBeforeMount, ref } from 'vue';
import { Loading, Notify } from 'quasar'
import { useRoute, useRouter } from 'vue-router';
import axios from 'axios';
import BugViewModel from '@/viewmodels/BugViewModel';

const props = defineProps({
  Bug: BugViewModel
});

//Options for bug severity
const options = ["Low", "Medium", "High"];

//Stores bug summary
const bugSummary = ref("");

//Stores bug severity
const bugSeverity = ref(options[Severity.Low]);

//Stores bug description
const bugDescription = ref("");

const route = useRoute();
const router = useRouter();

//Populates values if bug is being edited
onBeforeMount(() =>{
  if(!!props.Bug){
    bugSummary.value = props.Bug.summary;
    bugSeverity.value = options[props.Bug.severity];
    bugDescription.value = props.Bug.description;

  }
});

//Submits the form to the backend
async function onSubmit(){

  //Shows loading control
  Loading.show({
    message: "Please wait..."
  });

  //Assigns values to DTO
  const bugDTO = new BugDTO();

  bugDTO.summary = bugSummary.value;
  bugDTO.severity = options.indexOf(bugSeverity.value);
  bugDTO.description = bugDescription.value;

  try{
    //block for editing an existing bug
    if(!!props.Bug){
      bugDTO.bugID = route.params.bugId.toString();
      bugDTO.projectID = props.Bug.projectID;

      await axios.patch("/bugs/updatebug", bugDTO);

      //Notification if bug updates are saved
      Notify.create({
        message: "Successfully saved changes.",
        position: "bottom",
      type: "positive"
  });

    }
    //Block for adding new bug
    else{
      bugDTO.projectID = route.params.projectId.toString();

      await axios.post("/bugs/addbug", bugDTO);
      await router.push({path:`/project/${route.params.projectId}/bugs/`});
    }
  }
  catch {
    Notify.create({
      message: "Something went wrong when processing your request. Please try again later.",
      position: "bottom",
      type: "negative"
  });
}

  Loading.hide();
}
</script>
