// Stores a list of bug comments, as well as the total number
// of comments and its current page.

import type CommentViewModel from "./CommentViewModel";

export default class CommentContainer{

  // The total number of comments the bug has.
  totalComments: number = 0;

  // The current page of commennts the user is on.
  currentPage: number = 1;

  // Stores the comments that will be seen by the clients.
  comments: CommentViewModel[] = []
}
