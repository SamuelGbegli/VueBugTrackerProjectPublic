<template>
  <div class="row">
    <QSpace/>
    <!--Shows when the comment was posted. If the user edited it "(Edited)" is also shown-->
    <h6>{{ comment?.edited? `${formatDate(comment.datePosted)} (Edited)` : comment? `${formatDate(comment.datePosted)}` : ""}}</h6>
  </div>
  <QCard>
    <QCardSection>
    <!--Shows username and icon of comment poster-->
      <UserIcon :username="props.comment?.ownerName" :icon="props.comment?.ownerIcon"/>
      <!--Shows a preview of the comment being replied to-->
      <QCard v-if="!!props.comment?.replyCommentID">
        <QCardSection>
          <UserIcon :username="props.comment.replyCommentOwnerName" :icon="props.comment?.replyCommentOwnerIcon"/>
          <p>{{ props.comment.replyCommentText }}</p>
        </QCardSection>
      </QCard>
      <!--Message if comment being replied to has been deleted-->
      <QBanner v-if="props.comment?.isCommentReplyDeleted">
        The original comment has been deleted.
      </QBanner>
      <p>{{ props.comment?.text }}</p>
    </QCardSection>
  </QCard>
</template>
<script setup lang="ts">
import CommentViewModel from '@/viewmodels/CommentViewModel';
import UserIcon from './UserIcon.vue';
import formatDate from '@/classes/helpers/FormatDate';

const props = defineProps({
  //The comment being bound
  comment: CommentViewModel
});

</script>
