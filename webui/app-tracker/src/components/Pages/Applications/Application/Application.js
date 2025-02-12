import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import Menu from "../../../Menu/Menu";
import "../../../../App.css";
import "./Application.css";

export default function Application() {
  const { id } = useParams();
  const [application, setApplication] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function fetchApplication() {
      try {
        const response = await fetch(
          `http://localhost:5000/api/applications/${id}`
        );
        if (!response.ok) {
          throw new Error("Failed to fetch application");
        }
        const data = await response.json();
        setApplication(data);
      } catch (error) {
        setError(error.message);
      } finally {
        setLoading(false);
      }
    }
    fetchApplication();
  }, [id]);

  return (
    <div className="app">
      <Menu />
      <h2>Application Info</h2>
      {loading ? (
        <p>Loading....</p>
      ) : error ? (
        <p>Error {error}</p>
      ) : !application ? (
        <h3>Not Found</h3>
      ) : (
        <div className="application">
          <div>"id": {application.id}</div>
          <div>"applicationDate": {application.applicationDate}</div>
          <div>"source": {application.source.name}</div>
          <div>"organization": {application.organization.name}</div>
          <div>"jobTitle": {application.jobTitle.name}</div>
          <div>"workEnvironment": {application.workEnvironment.name}</div>
          <div>"city": {application.city}</div>
          <div>"state": {application.state}</div>
          <div></div>
        </div>
      )}
    </div>
  );
}
