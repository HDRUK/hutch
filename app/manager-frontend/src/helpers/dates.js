import { getDate } from "date-fns";

export const getTimeHoursAgo = () => {
  // gets the date a specified amount of days ago
  var d = new Date();
  d.setHours(d.getHours() - 2);
  return d;
};

const getDateString = (date, thisMonth = false, thisYear = false) => {
  const options = {
    year: "numeric",
    month: "long",
    day: "numeric",
  };
  const dateString = date.toLocaleString("default", options);
  const hours = date.getHours();
  let minutes = date.getMinutes();

  if (minutes < 10) {
    //add leading 0 to minutes
    minutes = `0${minutes}`;
  }
  if (thisMonth) {
    return `${thisMonth} at ${hours}:${minutes}`;
  }
  if (thisYear) {
    return `${dateString}  at ${hours}:${minutes}`;
  }
  return `${dateString}. at ${hours}:${minutes}`;
};

export const getTimeAgo = (date) => {
  var d = new Date(date);

  const dayMs = 1000 * 60 * 60 * 24; //1 day in ms
  const today = new Date();
  const yesterday = new Date(today - dayMs);
  const seconds = Math.round((today - d) / 1000);
  const minutes = Math.round(seconds / 60);
  const isToday = today.toDateString() === d.toDateString();
  const isYesterday = yesterday.toDateString() === d.toDateString();
  const isThisYear = today.getFullYear() === d.getFullYear();

  if (seconds < 5) {
    return "now";
  } else if (seconds < 60) {
    return "less than a minute ago";
  } else if (seconds < 90) {
    return "about a minute ago";
  } else if (minutes < 60) {
    return `${minutes} minutes ago`;
  } else if (isToday) {
    return getDateString(d, "Today");
  } else if (isYesterday) {
    return getDateString(d, "Yesterday");
  } else if (isThisYear) {
    return getDateString(d, false, isThisYear);
  }
  return getDateString(d, false, false);
};
