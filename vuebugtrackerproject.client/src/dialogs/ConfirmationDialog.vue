<template>
  <QDialog ref="dialogRef"
  persistent
  @hide="onDialogHide">
    <QCard class="q-dialog-plugin">
    <QForm @submit="onSubmit">
      <QCardSection>
        <h5>{{ props.header }}</h5>
      </QCardSection>
      <QCardSection>
        <p>{{ props.message }}</p>
        <QCheckbox
         v-model="confirmation"
         v-if="props.requiresConfirmation"
         label="I have read and understood the above message"
        />
      </QCardSection>
      <QCardActions align="right">
        <QBtn @click="onCloseClick" label="No"/>
        <QBtn type="submit" :disable="props.requiresConfirmation && !confirmation" label="Yes"/>
      </QCardActions>
    </QForm>
    </QCard>
  </QDialog>
</template>
<script setup lang="ts">

import { useDialogPluginComponent } from 'quasar';
import { ref } from 'vue';

const props = defineProps({
  requiresConfirmation: Boolean,
  header: String,
  message: String,
});

const confirmation = ref(false);

defineEmits([
  ...useDialogPluginComponent.emits
]);

const {dialogRef, onDialogHide, onDialogOK, onDialogCancel} = useDialogPluginComponent();

function onCloseClick(){
  onDialogCancel();
}

function onSubmit(){
  onDialogOK();
}

</script>
