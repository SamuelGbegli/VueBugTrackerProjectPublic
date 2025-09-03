
import type BugPreviewViewModel from "./BugPreviewViewModel";
//Stores a list of bug previews for a project, as well as the number of bugs
//that match a filter query and the page of filtered bugs being accessed.
export default class BugPreviewContainer {
  // The total number of bugs based on the filter given.
  numberOfBugs: number = 0;
  // The current page of filtered bugs being viewed.
  currentPage: number = 1;
  // The list of bug previews that will be shown to the user.
  bugPreviews: BugPreviewViewModel[] = []
}
