//Determines whether a bug is closed or open.
const Status = {
  // The bug is fixed, or no more work will be performed on it.
  Closed: 0,
  // The bug is currently open, and work is expected to be performed on it.
  Open: 1
} as const;

export default Status;
