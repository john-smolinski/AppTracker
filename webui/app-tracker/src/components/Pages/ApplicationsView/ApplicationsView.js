import React, { useEffect } from "react";
import { useDispatch } from "react-redux";
import { fetchSources } from "../../../redux/sourcesSlice";
import { fetchJobTitles } from "../../../redux/jobTitlesSlice";
import { fetchWorkEnvironments } from "../../../redux/workEnvironmentsSlice";
import Menu from "../../Menu/Menu";

export default function ApplicationsView() {
  const dispatch = useDispatch();

  useEffect(() => {
    // fetch supporting data on start
    dispatch(fetchSources());
    dispatch(fetchJobTitles());
    dispatch(fetchWorkEnvironments());
  }, [dispatch]);

  return (
    <div className="app">
      <Menu />
      <h1>Search Placeholder</h1>
    </div>
  );
}
