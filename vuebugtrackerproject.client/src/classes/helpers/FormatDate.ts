//Converts a date fetched from the backend to a more "user friendly" format

import { date } from "quasar";

export default function formatDate(value : Date){
  return date.formatDate(new Date(value), "DD/MM/YYYY HH:mm:ss");
}
