<template>
  <br/>
  <div class="q-gutter-md">
  <div class="row">
    <h6>Total comments: {{ commentContainer.totalComments }}</h6>
    <QSpace/>
    <QBtn @click="showCommentDialog()" v-if="addCommentStatusCode === 200" label="Add comment"/>
  </div>
  <br/>
  <div v-if="!commentStatusCode || commentStatusCode == 200">
    <div v-for="x in commentContainer.comments" v-bind:key="(x as CommentViewModel).id">
      <CommentPreview :comment="(x as CommentViewModel)" />
      <div class="row" v-if="!x.isStatusUpdate && addCommentStatusCode === 200">
        <QSpace/>
        <QBtn v-if="authStore.getUserID()" @click="showCommentDialog(null, x)" label="Reply" />
        <QBtn v-if="x.ownerID === authStore.getUserID()" @click="showCommentDialog(x)" label="Edit" />
        <QBtn v-if="x.ownerID === authStore.getUserID()" @click="deleteComment(x.id)" label="Delete" />
      </div>
      <br/>
    </div>
    <div class="row">
      <QSpace/>
      <QPagination v-model="currentPage"
                   :min="1"
                   :max="numberOfPages"
                   @update:model-value="getComments()"
                   input />
    </div>
      <QInnerLoading
        :showing="!commentStatusCode"
        style="height: 100%;"
        label="Loading..."/>

  </div>
  <div v-else>
    <h5>Failed to load data.</h5>
    <QBtn @click="getComments()" label="Reload"/>
  </div>
  </div>
</template>
<script setup lang="ts">
  import CommentPreview from '@/components/CommentPreview.vue';
  import CommentContainer from '@/viewmodels/CommentContainer';
  import CommentViewModel from '@/viewmodels/CommentViewModel';
  import axios, { AxiosError } from 'axios';
  import { ref, onBeforeMount } from 'vue';
  import { useRoute } from 'vue-router';
  import { useAuthStore } from '@/stores/AuthStore';
  import { Dialog, Notify } from 'quasar';
  import CommentDialog from '@/dialogs/CommentDialog.vue';
  import ConfirmationDialog from '@/dialogs/ConfirmationDialog.vue';

  //The page of comments the user is on
  const currentPage = ref(1);

  //The number of pages of comments the bug has
  const numberOfPages = ref(5);

  //Stores the number of comments, and the current page and comments visible
  const commentContainer = ref(new CommentContainer());

  //Stores the status code when fetching comments
  const commentStatusCode = ref();

  //Stores status code for whether the user add a comment
  const addCommentStatusCode = ref();

  const route = useRoute();
  const authStore = useAuthStore();

  onBeforeMount(async () => {
    await getComments();
    await canUserAddComments()
    if (!!commentContainer.value) {
      numberOfPages.value = Math.ceil(commentContainer.value.totalComments / 20);
    }
  });

  //Loads comments from server
  async function getComments() {
    commentStatusCode.value = null;
    try {
      const response = await axios.get(`/comments/get/${route.params.bugId}/${currentPage.value}`);
      commentContainer.value = Object.assign(new CommentContainer, response.data);
      numberOfPages.value = Math.ceil(commentContainer.value.totalComments / 20);

      //Ensures comment page goes back if the last comment in a page is removed
      if(currentPage.value > numberOfPages.value)
        currentPage.value = numberOfPages.value;

      commentStatusCode.value = response.status
    } catch(ex) {
      //Frontend failed to fetch comments, shows error message
      const error = ex as AxiosError
      commentStatusCode.value = error.status
    }
  };

  //Shows the comment dialog
  function showCommentDialog(comment: CommentViewModel | null = null, reply: CommentViewModel | null = null){
    Dialog.create({
      component: CommentDialog,
      componentProps:{
        existingComment: comment,
        replyComment: reply
      }
    }).onOk(async () => {

      //Goes to last page if comment is being added
      if(!comment)
        currentPage.value = Math.ceil(Math.ceil((commentContainer.value.totalComments + 1) / 20))

        //Reloads comments
      await getComments();

        //Notifies the user a new comment has been added or edited
        Notify.create({
    message: !!comment ? "Successfully edited comment.": "Successfully added comment.",
    position: "bottom",
    type: "positive"
    });
    });
  }

  //Removes a comment
  function deleteComment(commentId: string){
    Dialog.create({
      component: ConfirmationDialog,
      componentProps:{
        requiresConfirmation: false,
        header: "Delete comment",
        message: "Comments cannot be recovered once deleted. Are you sure you want to continue?"
      }
    }).onOk(async () =>{
      try{
        //Sends delete request to comment
        await axios.delete("/comments/delete", {
        headers:{
          "Content-Type": "application/json"
        },
        data: commentId
      });

      //Goes back one page if last comment on page has been deleted
      if(Math.ceil((commentContainer.value.totalComments - 1) / 20))
        currentPage.value--;

      //Gets new set of comments
      await getComments();

      Notify.create({
        message: "Successfully deleted comment.",
        position: "bottom",
        type: "positive"
        });

      }
      catch{
        Notify.create({
        message: "Something went wrong when processing your request. Please try again later.",
        position: "bottom",
        type: "negative"
        });
      }
    });
  }

  //Function to verify if the user can edit the project's bug
async function canUserAddComments(){
  try{
    const response = await axios.get(`/comments/canadd/${route.params.bugId}`);
    addCommentStatusCode.value = response.status;
  }
  catch (ex) {
    const error = ex as AxiosError;
    addCommentStatusCode.value = error.status;
  }
}
</script>
