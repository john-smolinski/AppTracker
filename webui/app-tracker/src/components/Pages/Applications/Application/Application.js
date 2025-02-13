import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { useParams } from "react-router-dom";
import { FormControl, FormLabel, Box } from "@mui/material";
import Menu from "../../../Menu/Menu";
import "../../../../App.css";
import "./Application.css";

export default function Application() {
  const { id } = useParams();
  const [application, setApplication] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const applications = useSelector((state) => state.applications.apps);

  // city and state options
  const [city, setCity] = useState("");
  const [state, setState] = useState("");
  const [cityOptions, setCityOptions] = useState([]);
  const [stateOptions, setStateOptions] = useState([]);

  useEffect(() => {
    if (applications.length > 0) {
      const uniqueCities = [
        ...new Set(applications.map((app) => app.city).filter(Boolean)),
      ];
      const uniqueStates = [
        ...new Set(applications.map((app) => app.state).filter(Boolean)),
      ];

      setCityOptions(uniqueCities);
      setStateOptions(uniqueStates);
    }
  }, [applications]);

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
          <Box display="flex" flexDirection="column" gap={2}>
            {/* non editable id labels */}
            <Box display="flex" alignItems="center">
              <Box flex="0 0 150px">
                <FormControl>
                  <FormLabel>id:</FormLabel>
                </FormControl>
              </Box>
              <Box flex="1">
                <FormControl>
                  <FormLabel>{application.id}</FormLabel>
                </FormControl>
              </Box>
            </Box>
            {/* non editable application date */}
            <Box display="flex" alignItems="center">
              <Box flex="0 0 150px">
                <FormControl>
                  <FormLabel>Application Date:</FormLabel>
                </FormControl>
              </Box>
              <Box flex="1">
                <FormControl>
                  <FormLabel>{application.applicationDate}</FormLabel>
                </FormControl>
              </Box>
            </Box>
            {/* non editable source */}
            <Box display="flex" alignItems="center">
              <Box flex="0 0 150px">
                <FormControl>
                  <FormLabel>Source:</FormLabel>
                </FormControl>
              </Box>
              <Box flex="1">
                <FormControl>
                  <FormLabel>{application.source.name}</FormLabel>
                </FormControl>
              </Box>
            </Box>
            {/* non editable organization */}
            <Box display="flex" alignItems="center">
              <Box flex="0 0 150px">
                <FormControl>
                  <FormLabel>Organization:</FormLabel>
                </FormControl>
              </Box>
              <Box flex="1">
                <FormControl>
                  <FormLabel>{application.organization.name}</FormLabel>
                </FormControl>
              </Box>
            </Box>
            {/* TODO update job title control */}
            <Box display="flex" alignItems="center">
              <Box flex="0 0 150px">
                <FormControl>
                  <FormLabel>Job Title:</FormLabel>
                </FormControl>
              </Box>
              <Box flex="1">
                <FormControl>
                  <FormLabel>{application.jobTitle.name}</FormLabel>
                </FormControl>
              </Box>
            </Box>
            {/* TODO update environment control */}
            <Box display="flex" alignItems="center">
              <Box flex="0 0 150px">
                <FormControl>
                  <FormLabel>Work Environment:</FormLabel>
                </FormControl>
              </Box>
              <Box flex="1">
                <FormControl>
                  <FormLabel>{application.workEnvironment.name}</FormLabel>
                </FormControl>
              </Box>
            </Box>
            {/* TODO update city control */}
            <Box display="flex" alignItems="center">
              <Box flex="0 0 150px">
                <FormControl>
                  <FormLabel>City:</FormLabel>
                </FormControl>
              </Box>
              <Box flex="1">
                <FormControl>
                  <FormLabel>{application.city}</FormLabel>
                </FormControl>
              </Box>
            </Box>
            {/* TODO update city control */}
            <Box display="flex" alignItems="center">
              <Box flex="0 0 150px">
                <FormControl>
                  <FormLabel>State:</FormLabel>
                </FormControl>
              </Box>
              <Box flex="1">
                <FormControl>
                  <FormLabel>{application.state}</FormLabel>
                </FormControl>
              </Box>
            </Box>
          </Box>
        </div>
      )}
    </div>
  );
}
