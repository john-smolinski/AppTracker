import React from "react";
import { useSelector } from "react-redux";
import styles from "./Summary.module.css";

export default function Summary() {
  const { apps, loading, error, isServerError } = useSelector(
    (state) => state.applications
  );

  // organize application counts by job posting source
  const sourceCounts = apps.reduce((accumulator, currentValue) => {
    const sourceName = currentValue.source.name;
    accumulator[sourceName] = (accumulator[sourceName] || 0) + 1;
    return accumulator;
  }, {});

  // organize application counts by working environment
  const environmentCounts = apps.reduce((accumulator, currentValue) => {
    const environmentName = currentValue.workEnvironment.name;
    accumulator[environmentName] = (accumulator[environmentName] || 0) + 1;
    return accumulator;
  }, {});

  // organzize applicatons counts by job title
  const jobTitleCounts = apps.reduce((accumulator, currentValue) => {
    const jobTitle = currentValue.jobTitle.name;
    accumulator[jobTitle] = (accumulator[jobTitle] || 0) + 1;
    return accumulator;
  }, {});

  let topJobTitles = Object.entries(jobTitleCounts).sort((a, b) => b[1] - a[1]);

  if (topJobTitles.length > 5) {
    topJobTitles = topJobTitles.slice(0, 5);
  }

  if (loading)
    return (
      <div className={styles["summary"]}>
        <p>Loading...</p>
      </div>
    );
  if (isServerError)
    return (
      <div>
        <h3 className={styles["summary"]}>
          Server error occurred. Please try again later.
        </h3>
      </div>
    );
  if (error)
    return (
      <div className={styles["summary"]}>
        <h1>Error:</h1> <p> {error.message}</p>
      </div>
    );
  return (
    <div>
      <h2>Summary</h2>
      <div className={styles["summary"]}>
        <h2>{apps.length} Total Applications Submitted </h2>
        <div className={styles["content-container"]}>
          <div className={styles["box"]}>
            <h2>Applications by Source</h2>
            <ul>
              {Object.entries(sourceCounts).map(([key, value]) => (
                <li key={key}>
                  {key} : {value}
                </li>
              ))}
            </ul>
          </div>
          <div className={styles["box"]}>
            <h2>Applications for Location Type</h2>
            <ul>
              {Object.entries(environmentCounts).map(([key, value]) => (
                <li key={key}>
                  {key} : {value}
                </li>
              ))}
            </ul>
          </div>
          <div className={styles["box"]}>
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
