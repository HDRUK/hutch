export const getDateDaysAgo = (days) => {
  // gets the date a specified amount of days ago
  var d = new Date();
  d.setDate(d.getDate() - days);
  return d;
};
