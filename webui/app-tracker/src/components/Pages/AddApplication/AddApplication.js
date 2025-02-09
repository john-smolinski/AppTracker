import React, { useState, useEffect, useMemo } from "react";
import { useSelector, useDispatch } from "react-redux";
import { postApplication } from "../../../redux/applicationsSlice";
import { fetchSources } from "../../../redux/sourcesSlice";
import { fetchOrganizations } from "../../../redux/organizataionsSlice";
import { fetchJobTitles } from "../../../redux/jobTitlesSlice";
import { fetchWorkEnvironments } from "../../../redux/workEnvironmentsSlice";
import dayjs from "dayjs";
import {
  FormControl,
  FormLabel,
  Box,
  Stack,
  Button,
  Snackbar,
  Alert,
} from "@mui/material";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import Menu from "../../Menu/Menu";
import AutoCompleteField from "../../Features/AutoCompleteField/AutoCompleteField";
import "./AddApplication.css";

export default function AddApplication() {
  // access autocomplete values from the Redux state
  const sources = useSelector((state) => state.appSources.sources);
  const organizations = useSelector(
    (state) => state.organizations.organizations
  );
  const jobTitles = useSelector((state) => state.jobTitles.titles);
  const environments = useSelector(
    (state) => state.workEnvironments.environments
  );
  const applications = useSelector((state) => state.applications.apps);

  // city and state options
  const [city, setCity] = useState("");
  const [state, setState] = useState("");
  const [cityOptions, setCityOptions] = useState([]);
  const [stateOptions, setStateOptions] = useState([]);

  // application properties
  const [selectedSource, setSelectedSource] = useState(null);
  const [selectedOrganization, setSelectedOrganization] = useState(null);
  const [selectedTitle, setSelectedTitle] = useState(null);
  const [selectedEnvironment, setSelectedEnvironment] = useState(null);

  // application date
  const [appDate, setAppDate] = useState(dayjs(new Date()));

  // validation and reset states
  const [isValid, setIsValid] = useState(false);
  const [resetKey, setResetKey] = useState(0);

  // snackbar states
  const [openSnackbar, setOpenSnackbar] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState("");
  const [snackbarSeverity, setSnackbarSeverity] = useState("success");

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

  // the new application to post
  const newApplication = useMemo(
    () => ({
      applicationDate: appDate,
      source: {
        id: sources.find((s) => s.name === selectedSource)?.id || null,
        name: selectedSource || null,
      },
      organization: {
        id:
          organizations.find((o) => o.name === selectedOrganization)?.id ||
          null,
        name: selectedOrganization || null,
      },
      jobTitle: {
        id: jobTitles.find((j) => j.name === selectedTitle)?.id || null,
        name: selectedTitle || null,
      },
      workEnvironment: {
        id:
          environments.find((w) => w.name === selectedEnvironment)?.id || null,
        name: selectedEnvironment || null,
      },
      city: city != null && city.length > 0 ? city : null,
      state: state != null && state.length === 2 ? state : null,
    }),
    [
      appDate,
      selectedSource,
      selectedOrganization,
      selectedTitle,
      selectedEnvironment,
      city,
      state,
      sources,
      organizations,
      jobTitles,
      environments,
    ]
  );

  useEffect(() => {
    setIsValid(
      appDate != null &&
        selectedSource !== null &&
        selectedSource.trim() !== "" &&
        selectedOrganization !== null &&
        selectedOrganization.trim() !== "" &&
        selectedTitle !== null &&
        selectedTitle.trim() !== "" &&
        selectedEnvironment !== null &&
        selectedEnvironment.trim() !== ""
    );
  }, [
    appDate,
    selectedSource,
    selectedOrganization,
    selectedTitle,
    selectedEnvironment,
  ]);

  // generic function to handle change events for autocomplete changes
  const handleChange = (setter) => (_, newValue) => {
    if (typeof newValue === "string") {
      setter(newValue); // use the typed value
    } else if (newValue?.inputValue) {
      setter(newValue.inputValue); // handle the manual input value
    } else {
      setter(newValue); // use the selected dropdown value
    }
  };

  // make sure values are updated when the field loses focus
  const handleBlur = (setter, value) => () => {
    if (value && value.trim() !== "") {
      setter(value.trim());
    }
  };

  const filteredOptions = (options, params) => {
    const filtered = options.filter((option) =>
      option.toLowerCase().includes(params.inputValue.toLowerCase())
    );
    // Add the user input as a new option if it doesn't exists
    if (params.inputValue !== "" && !options.includes(params.inputValue)) {
      filtered.push({
        inputValue: params.inputValue,
        label: `Add ${params.inputValue}`,
      });
    }
    return filtered;
  };

  const handleReset = () => {
    setAppDate(dayjs(new Date()));
    setSelectedSource(null);
    setSelectedOrganization(null);
    setSelectedTitle(null);
    setSelectedEnvironment(null);
    setCity(null);
    setState(null);
    setResetKey((prev) => prev + 1);
  };

  const dispatch = useDispatch();

  const handleSubmit = async () => {
    try {
      const response = await dispatch(postApplication(newApplication)).unwrap();
      if (response) {
        setSnackbarMessage(
          `Application for ${response.jobTitle.name} at ${response.organization.name} added`
        );
        setSnackbarSeverity("success");
        setOpenSnackbar(true);

        // update the state of any related item that may have been added as part of posting a new application
        if (
          response?.source &&
          !sources.find((s) => s.name === response.source.name)
        ) {
          dispatch(fetchSources());
        }
        if (
          response?.organization &&
          !organizations.find((o) => o.name === response.organization.name)
        ) {
          dispatch(fetchOrganizations());
        }
        if (
          response?.jobTitle &&
          !jobTitles.find((j) => j.name === response.jobTitle.name)
        ) {
          dispatch(fetchJobTitles());
        }
        if (
          response?.workEnvironment &&
          !environments.find((e) => e.name === response.workEnvironment.name)
        ) {
          dispatch(fetchWorkEnvironments());
        }
        // reset the form
        handleReset();
      }
    } catch (error) {
      console.log(error);
      setSnackbarMessage("Error adding application.");
      setSnackbarSeverity("error");
      setOpenSnackbar(true);
    }
  };

  return (
    <div className="app">
      <Menu />
      <h2>Add Application</h2>
      {/* outter boundry */}
      <div className="add-application">
        {/* box enclosing form */}
        <Box display="flex" flexDirection="column" gap={2}>
          {/* Box for date picker*/}
          <Box display="flex" alignItems="center">
            <Box flex="0 0 150px">
              <FormControl>
                <FormLabel>Application Date:</FormLabel>
              </FormControl>
            </Box>
            <Box flex="1">
              <LocalizationProvider dateAdapter={AdapterDayjs}>
                <DatePicker
                  value={appDate}
                  onChange={(newValue) => setAppDate(newValue)}
                />
              </LocalizationProvider>
            </Box>
          </Box>
          {/* select/add source control */}
          <AutoCompleteField
            resetKey={resetKey}
            sx={{ width: 300 }}
            value={selectedSource}
            onChange={handleChange(setSelectedSource)}
            onBlur={handleBlur(setSelectedSource, selectedSource)}
            filteredOptions={filteredOptions}
            options={sources.map((x) => x.name)}
            freeSolo
            label="Source"
          />
          {/* select/add organization control */}
          <AutoCompleteField
            resetKey={resetKey}
            sx={{ width: 300 }}
            value={selectedOrganization}
            onChange={handleChange(setSelectedOrganization)}
            onBlur={handleBlur(setSelectedOrganization, selectedOrganization)}
            filteredOptions={filteredOptions}
            options={organizations.map((x) => x.name)}
            freeSolo
            label="Organization"
          />

          {/* select/add job title control */}
          <AutoCompleteField
            resetKey={resetKey}
            sx={{ width: 300 }}
            value={selectedTitle}
            onChange={handleChange(setSelectedTitle)}
            onBlur={handleBlur(setSelectedTitle, selectedTitle)}
            filteredOptions={filteredOptions}
            options={jobTitles.map((x) => x.name)}
            freeSolo
            label="Job Title"
          />

          {/* select/add environment control */}
          <AutoCompleteField
            resetKey={resetKey}
            sx={{ width: 300 }}
            value={selectedEnvironment}
            onChange={handleChange(setSelectedEnvironment)}
            onBlur={handleBlur(setSelectedEnvironment, selectedEnvironment)}
            filteredOptions={filteredOptions}
            options={environments.map((x) => x.name)}
            freeSolo
            label="Work Environment"
          />

          {/* select/add city control */}
          <AutoCompleteField
            resetKey={resetKey}
            sx={{ width: 300 }}
            value={city}
            onChange={handleChange(setCity)}
            onBlur={handleBlur(setCity, city)}
            filteredOptions={filteredOptions}
            options={cityOptions}
            freeSolo
            label="City"
          />

          {/* select/add state control */}
          <AutoCompleteField
            resetKey={resetKey}
            sx={{ width: 300 }}
            value={state}
            onChange={handleChange(setState)}
            onBlur={handleBlur(setState, state)}
            filteredOptions={filteredOptions}
            options={stateOptions}
            freeSolo
            label="State"
          />

          <Box>
            <Box flex="0 0 150px"></Box>
            <Box flex="1">
              <Stack direction="row" spacing={2}>
                <Button variant="contained" onClick={handleReset}>
                  Reset
                </Button>
                <Button
                  variant="contained"
                  disabled={!isValid}
                  onClick={handleSubmit}
                >
                  Submit
                </Button>
              </Stack>
            </Box>
          </Box>
        </Box>
      </div>
      {/* Snackbar Notification */}
      <Snackbar
        open={openSnackbar}
        autoHideDuration={5000}
        onClose={() => setOpenSnackbar(false)}
        anchorOrigin={{ vertical: "top", horizontal: "center" }}
      >
        <Alert
          onClose={() => setOpenSnackbar(false)}
          severity={snackbarSeverity}
          sx={{ width: "100%" }}
        >
          {snackbarMessage}
        </Alert>
      </Snackbar>
    </div>
  );
}
