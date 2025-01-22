import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import {
  fetchApplications,
  postApplication,
} from "../../../redux/applicationSlice";

export default function ApplicationsList() {
  const dispatch = useDispatch();
  const { items, loading, error, isServerError } = useSelector(
    (state) => state.applications
  );

  useEffect(() => {
    dispatch(fetchApplications());
  }, [dispatch]);

  // TODO: this code will move to an add/update application component
  // function handleAddApplication() {
  //   const newApplication = {
  //     applicationDate: new Date().toISOString().split("T")[0],
  //     source: { id: null, name: "Source Example" },
  //     organization: { id: null, name: "Organization Example" },
  //     jobTitle: { id: null, name: "Job Title Example" },
  //     workEnvironment: { id: null, name: "Remote Example" },
  //   };
  //   dispatch(postApplication(newApplication));
  // }

  if (loading) return <p>Loading...</p>;
  if (isServerError)
    return <p>Server error occurred. Please try again later.</p>;
  if (error) return <p>Error: {error.message}</p>;

  return (
    <div>
      <h1>Applications</h1>
      {loading && <p>Loading...</p>}
      {error && <p>Error: {error}</p>}
      <ul>
        {items.map((application) => (
          <li key={application.id}>
            {application.id}: {application.source?.name} at{" "}
            {application.organization?.name}
          </li>
        ))}
      </ul>
    </div>
  );
}
