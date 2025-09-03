<template>
<div class="q-pa-md row q-gutter-md">
  <!--Left hand side of page, contains description-->
  <div class="col-8">
    <QCard style="min-height: 60vh">
      <QCardSection v-if="statusCode === 200">
        <h5>Description</h5>
        <div v-html="(project.description != '' ? project.description : 'No description provided')"/>
      </QCardSection>
      <QCardSection v-else>
        <QSkeleton square height="500px"/>
      </QCardSection>
    </QCard>
  </div>
  <!--Right hand side of page, contains project information-->
  <div class="col">
    <QCard>
      <div v-if="statusCode === 200">
        <QCardSection>
        <h5>Project details</h5>
      </QCardSection>
      <QCardSection>
        <h6>Date created</h6>
        <span>{{ formatDate(project.dateCreated) }}</span>
      </QCardSection>
      <QCardSection>
        <h6>Owner</h6>
        <UserIcon :username="project.ownerName" :icon="project.ownerIcon"/>
      </QCardSection>
      <QCardSection>
        <h6>Project link</h6>
        <a v-if="project.link != ''" :href="project.link">{{ project.link }}</a>
        <span v-else>None provided</span>
      </QCardSection>
      <QCardSection>
        <h6>Number of bugs</h6>
        <span>{{ project.totalBugs }} ({{ project.openBugs }} open)</span>
      </QCardSection>
      <QCardSection>
        <h6>Tags</h6>
        <div v-if="project.tags.length > 0">
          <QChip color="blue"  v-for="x in project.tags" v-bind:key="project.tags.indexOf(x)">
            {{ x }}
          </QChip>
        </div>
        <div v-else>
          <span>No tags provided</span>
        </div>
      </QCardSection>
      </div>
      <div v-else>
        <QCardSection>
          <QSkeleton square height="500px"/>
        </QCardSection>
      </div>
    </QCard>
  </div>
</div>
</template>
<script setup lang="ts">

import axios, { AxiosError } from 'axios';
import { onBeforeMount, ref } from 'vue';
import { useRoute } from 'vue-router';
import ProjectViewModel from '@/viewmodels/ProjectViewModel';
import UserIcon from '@/components/UserIcon.vue';
import formatDate from '@/classes/helpers/FormatDate';

  const project = ref(new ProjectViewModel());
  const statusCode = ref();
  const route = useRoute();

  onBeforeMount(async ()=>{
    try{
      const response = await axios.get(`/projects/get/${route.params.projectId}`);
      statusCode.value = response.status;
      project.value = response.data;

    }
    catch (ex){
      const error = ex as AxiosError;
      statusCode.value = error.status;
    }
  });
</script>
