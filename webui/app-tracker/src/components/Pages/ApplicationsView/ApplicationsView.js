import React, { useEffect } from "react";
import { useDispatch } from "react-redux";
import { fetchSources } from "../../../redux/sourcesSlice";
import { fetchJobTitles } from "../../../redux/jobTitlesSlice";
import { fetchWorkEnvironments } from "../../../redux/workEnvironmentsSlice";
import Menu from "../../Menu/Menu";
import "../../../App.css";
import "./ApplicationsView.css";

export default function ApplicationsView() {
  const dispatch = useDispatch();

  useEffect(() => {
    // fetch supporting data on start - may consider moving this to the App.js
    dispatch(fetchSources());
    dispatch(fetchJobTitles());
    dispatch(fetchWorkEnvironments());
  }, [dispatch]);

  return (
    <div className="app">
      <Menu />
      <h1>Applications Submitted</h1>
      <div className="applications">
        <div>filter and search bar</div>
        <div>
          <p>all applications listed here</p>
        </div>
      </div>
    </div>
  );
}
