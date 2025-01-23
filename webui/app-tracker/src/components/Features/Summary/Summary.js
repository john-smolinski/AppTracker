import React from "react";
import { useSelector } from "react-redux";
import "./Summary.css";
import { current } from "@reduxjs/toolkit";

export default function Summary() {
  const { items, loading, error, isServerError } = useSelector(
    (state) => state.applications
  );

  // organize application counts by job posting source
  const sourceCounts = items.reduce((accumulator, currentValue) => {
    const sourceName = currentValue.source.name;
    accumulator[sourceName] = (accumulator[sourceName] || 0) + 1;
    return accumulator;
  }, {});

  // organize application counts by working environment
  const environmentCounts = items.reduce((accumulator, currentValue) => {
    const environmentName = currentValue.workEnvironment.name;
    accumulator[environmentName] = (accumulator[environmentName] || 0) + 1;
    return accumulator;
  }, {});

  // organzize applicatons counts by job title
  const jobTitleCounts = items.reduce((accumulator, currentValue) => {
    const jobTitle = currentValue.jobTitle.name;
    accumulator[jobTitle] = (accumulator[jobTitle] || 0) + 1;
    return accumulator;
  }, {});

  let topJobTitles = Object.entries(jobTitleCounts).sort((a, b) => b[1] - a[1]);

  if (topJobTitles.length > 5) {
    topJobTitles = topJobTitles.slice(0, 5);
  }
  topJobTitles.forEach((itm) => console.log(itm));

  if (loading)
    return (
      <div className="summary">
        <p>Loading...</p>
      </div>
    );
  if (isServerError)
    return (
      <div>
        <h3 className="summary">
          Server error occurred. Please try again later.
        </h3>
      </div>
    );
  if (error)
    return (
      <div className="summary">
        <h1>Error:</h1> <p> {error.message}</p>
      </div>
    );
  return (
    <div>
      <h1>Summary View</h1>
      <div className="summary">
        <h3>{items.length} Total Applications submitted </h3>
        <div className="content-container">
          <div className="box">
            <h2>Applications by Source</h2>
            <ul>
              {Object.entries(sourceCounts).map(([key, value]) => (
                <li key={key}>
                  {key} : {value}
                </li>
              ))}
            </ul>
          </div>
          <div className="box">
            <h2>Applications for Location Type</h2>
            <ul>
              {Object.entries(environmentCounts).map(([key, value]) => (
                <li key={key}>
                  {key} : {value}
                </li>
              ))}
            </ul>
          </div>
          <div className="box">
            <h2>
              Top {topJobTitles.length} of{" "}
              {Object.entries(jobTitleCounts).length} Job Titles Applied For
            </h2>
            <ul>
              {topJobTitles.map((pair) => (
                <li key={pair[0]}>
                  {pair[0]} : {pair[1]}
                </li>
              ))}
            </ul>
          </div>
        </div>
      </div>
    </div>
  );
}
