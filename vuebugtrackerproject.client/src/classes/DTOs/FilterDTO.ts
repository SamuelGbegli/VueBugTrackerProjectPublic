export default class FilterDTO{

	// The sting that is used to search for projects.
	query: string = "";
	// Filters projects if they have any open bugs or not.
	projectType: number = 0;
	// Determines to search for a project based on the date it was
	// created or last updated.
	dateSearch: number = 0;
	// The starting date search range.
	dateFrom: string | null = null;
	// The end date search range.
	dateEnd: string | null = null;
	// Determines how the items will be sorted.
	sortType: number = 0;
	// Determines whether the items are sorted in ascending or
	// descending order.
	sortOrder: number = 0;
  // The page of projects that will be returned.
  pageNumber: number = 1;
}
