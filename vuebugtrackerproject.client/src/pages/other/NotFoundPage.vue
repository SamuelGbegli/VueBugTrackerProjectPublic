<template>
  <ErrorBanner :prompt-type="ErrorPromptType.GoBackButtonOnly"
      :message="'The page you requested does not exist.'"/>
</template>
<script setup lang="ts">
import ErrorBanner from '@/components/ErrorBanner.vue';
import ErrorPromptType from '@/enumConsts/ErrorPromptType';

</script>
