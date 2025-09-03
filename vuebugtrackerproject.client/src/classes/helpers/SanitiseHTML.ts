//Sanitises HTML content to remove tags.
//Based on this Stack Overflow question:
//https://stackoverflow.com/questions/2794137/sanitizing-user-input-before-adding-it-to-the-dom-in-javascript

export default function sanitiseHTML(input: string){
  const map: Record<string, string> = {
    '&' : '&amp',
    '<' : '&lt',
    '>' : '&gt',
    '"' : '&quot',
    "'" : '&#x27',
    '/' : '&#x2f',
  };

  const reg = /[&<>"'/]/ig;

  return input.replace(reg, (match) => (map[match]));
}
