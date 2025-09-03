import Severity from "@/enumConsts/Severity";
import Status from "@/enumConsts/Status";

// DTO for filtering a project's bug.
export default class BugFilterDTO{
  // Search query for the summary of the bug.
  summary: string = "";
  // Decides whether to filter bugs on date created or
  // last updated.
  dateSearch: number = 0;
  // The starting date search range.
	dateFrom: string | null = null;
	// The end date search range.
	dateEnd: string | null = null;
  // The username of whoever created the bug.
  creatorName: string = "";
  // Filters bugs based on severity. These are a set of
  // integers representing "low", "medium" and "high".
  severityValues: number[] = [Severity.Low, Severity.Medium, Severity.High];
  // Filters bugs based on whether they are open or closed.
  // These are a set of integers representing "open" and "closed".
  statusValues: number[] = [Status.Closed, Status.Open];
	// Determines how the items will be sorted.
	sortType: number = 0;
	// Determines whether the items are sorted in ascending or
	// descending order.
	sortOrder: number = 0;
  // The page of bugs that will be returned.
  pageNumber: number = 1;
  // The ID of the project the bugs belong to.
  projectID: string = "";
}
