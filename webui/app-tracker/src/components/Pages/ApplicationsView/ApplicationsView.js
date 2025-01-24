import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchSources } from "../../../redux/sourcesSlice";
import { fetchJobTitles } from "../../../redux/jobTitlesSlice";
import { fetchWorkEnvironments } from "../../../redux/workEnvironmentsSlice";
import Menu from "../../Menu/Menu";
import "../../../App.css";
import "./ApplicationsView.css";

export default function ApplicationsView() {
  const dispatch = useDispatch();

  // dispatch actions to get filter values
  useEffect(() => {
    // fetch supporting data on start - may consider moving this to the App.js
    dispatch(fetchSources());
    dispatch(fetchJobTitles());
    dispatch(fetchWorkEnvironments());
  }, [dispatch]);

  // filter values from state
  const sources = useSelector((state) => state.appSources.sources);
  const jobTitles = useSelector((state) => state.jobTitles.titles);
  const environments = useSelector(
    (state) => state.workEnvironments.environments
  );

  const [companyFilter, setCompanyFilter] = useState("");
  const [sourceFilter, setSourceFilter] = useState("");
  const [jobTitleFilter, setJobTitleFilter] = useState("");
  const [environmentFilter, setEnvironmentFilter] = useState("");

  return (
    <div className="app">
      <Menu />
      <h1>Applications Submitted</h1>
      <div className="applications">
        <h4>Filters</h4>
        <div>
          <label>Company Name:</label>
          <input
            type="text"
            placeholder="Search Company Name..."
            value={companyFilter}
            onChange={(e) => setCompanyFilter(e.target.value)}
            className="text-filter"
          />
          <label>Source:</label>
          <select
            value={sourceFilter}
            onChange={(e) => setSourceFilter(e.target.value)}
            className="select-filter"
          >
            <option value="">Select...</option>
            {sources.map((option) => (
              <option key={option.id} value={option.name}>
                {option.name}
              </option>
            ))}
          </select>
          <label>Job Title:</label>
          <select
            value={jobTitleFilter}
            onChange={(e) => setJobTitleFilter(e.target.value)}
            className="select-filter"
          >
            <option>Select...</option>
            {jobTitles.map((option) => (
              <option key={option.id} value={option.name}>
                {option.name}
              </option>
            ))}
          </select>
          <label>Location:</label>
          <select
            value={environmentFilter}
            onChange={(e) => setEnvironmentFilter(e.target.value)}
            className="select-filter"
          >
            <option>Select...</option>
            {environments.map((option) => (
              <option key={option.id} value={option.name}>
                {option.name}
              </option>
            ))}
          </select>
        </div>
        <div>
          <h5>filter values:</h5>
          <div>
            company = {companyFilter} | source = {sourceFilter} | jobTitle ={" "}
            {jobTitleFilter} | environment = {environmentFilter}
          </div>
        </div>
      </div>
    </div>
  );
}
