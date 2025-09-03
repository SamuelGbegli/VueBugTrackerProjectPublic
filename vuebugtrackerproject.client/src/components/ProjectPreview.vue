<!--Shows a preview of a project when browsing-->
<template>
  <QCard style="width:100%; max-width: 320px;">
    <QCardSection>
      <div class="text-h5">{{ props.Project?.name }}</div>
      <div class="text-subtitle2 elipsis">{{ props.Project?.summary }}</div>
    </QCardSection>
    <QCardSection>
      <UserIcon
        :username="Project?.ownerName"
        :icon="Project?.ownerIcon"/>
      </QCardSection>
      <QCardSection>
     <div class="text-subtitle2">Total bugs: {{ props.Project?.totalBugs }} ({{ props.Project?.openBugs }} open)</div>
      <div class="text-subtitle2">Last updated: {{ props.Project? formatDate(props.Project.dateModified) : "" }}</div>
    </QCardSection>
    <QCardActions align="right">
      <QBtn :to="`/project/${props.Project?.id}`" label="View"/>
    </QCardActions>
  </QCard>
</template>
<script setup lang="ts">

import ProjectPreviewViewModel from '@/viewmodels/ProjectPreviewViewModel';
import UserIcon from './UserIcon.vue';
import formatDate from '@/classes/helpers/FormatDate';

const props = defineProps({
  Project: ProjectPreviewViewModel
});

</script>
