import React, { useState, useEffect, useMemo } from "react";
import { useSelector, useDispatch } from "react-redux";
import dayjs from "dayjs";
import {
  Autocomplete,
  TextField,
  FormControl,
  FormLabel,
  Box,
  Stack,
  Button,
} from "@mui/material";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import Menu from "../../Menu/Menu";
import "./AddApplication.css";
import { postApplication } from "../../../redux/applicationsSlice";

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

  // application date
  const [appDate, setAppDate] = useState(dayjs(new Date()));

  // application properties
  const [selectedSource, setSelectedSource] = useState(null);
  const [selectedOrganization, setSelectedOrganization] = useState(null);
  const [selectedTitle, setSelectedTitle] = useState(null);
  const [selectedEnvironment, setSelectedEnvironment] = useState(null);

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
    }),
    [
      appDate,
      selectedSource,
      selectedOrganization,
      selectedTitle,
      selectedEnvironment,
      sources,
      organizations,
      jobTitles,
      environments,
    ]
  );

  const [isValid, setIsValid] = useState(false);

  useEffect(() => {
    setIsValid(
      appDate != null &&
        selectedSource != null &&
        selectedOrganization != null &&
        selectedTitle != null &&
        selectedEnvironment != null
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
    setSelectedSource(null);
    setSelectedOrganization(null);
    setSelectedTitle(null);
    setSelectedEnvironment(null);
  };

  const dispatch = useDispatch();
  const handleSubmit = () => {
    dispatch(postApplication(newApplication));
    handleReset();
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
          <Box display="flex" alignItems="center">
            <Box flex="0 0 150px">
              <FormControl>
                <FormLabel>Source:</FormLabel>
              </FormControl>
            </Box>
            <Box flex="1">
              <Autocomplete
                sx={{ width: 300 }}
                value={selectedSource}
                onChange={handleChange(setSelectedSource)}
                filterOptions={filteredOptions}
                selectOnFocus
                handleHomeEndKeys
                options={sources.map((x) => x.name)}
                getOptionLabel={(option) =>
                  typeof option === "string" ? option : option.label
                }
                renderOption={(props, option) => (
                  <li {...props}>
                    {typeof option === "string" ? option : option.label}
                  </li>
                )}
                freeSolo
                renderInput={(params) => (
                  <TextField
                    {...params}
                    label="Select or Add new"
                    variant="outlined"
                    className="auto-complete"
                  />
                )}
              />
            </Box>
          </Box>
          {/* select/add organization control */}
          <Box display="flex" alignItems="center">
            <Box flex="0 0 150px">
              <FormControl>
                <FormLabel>Organization:</FormLabel>
              </FormControl>
            </Box>
            <Autocomplete
              sx={{ width: 300 }}
              value={selectedOrganization}
              onChange={handleChange(setSelectedOrganization)}
              filterOptions={filteredOptions}
              selectOnFocus
              handleHomeEndKeys
              options={organizations.map((x) => x.name)}
              getOptionLabel={(option) =>
                typeof option === "string" ? option : option.label
              }
              renderOption={(props, option) => (
                <li {...props}>
                  {typeof option === "string" ? option : option.label}
                </li>
              )}
              freeSolo
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="Select or Add new"
                  variant="outlined"
                  className="auto-complete"
                />
              )}
            />
          </Box>

          {/* select/add job title control */}
          <Box display="flex" alignItems="center">
            <Box flex="0 0 150px">
              <FormControl>
                <FormLabel>Job title:</FormLabel>
              </FormControl>
            </Box>
            <Box flex="1">
              <Autocomplete
                sx={{ width: 300 }}
                value={selectedTitle}
                onChange={handleChange(setSelectedTitle)}
                filterOptions={filteredOptions}
                selectOnFocus
                handleHomeEndKeys
                options={jobTitles.map((x) => x.name)}
                getOptionLabel={(option) =>
                  typeof option === "string" ? option : option.label
                }
                renderOption={(props, option) => (
                  <li {...props}>
                    {typeof option === "string" ? option : option.label}
                  </li>
                )}
                freeSolo
                renderInput={(params) => (
                  <TextField
                    {...params}
                    label="Select or Add new"
                    variant="outlined"
                    className="auto-complete"
                  />
                )}
              />
            </Box>
          </Box>
          {/* select/add environment control */}
          <Box display="flex" alignItems="center">
            <Box flex="0 0 150px">
              <FormControl>
                <FormLabel>Environment:</FormLabel>
              </FormControl>
            </Box>
            <Box flex="1">
              <Autocomplete
                sx={{ width: 300 }}
                value={selectedEnvironment}
                onChange={handleChange(setSelectedEnvironment)}
                filterOptions={filteredOptions}
                selectOnFocus
                handleHomeEndKeys
                options={environments.map((x) => x.name)}
                getOptionLabel={(option) =>
                  typeof option === "string" ? option : option.label
                }
                renderOption={(props, option) => (
                  <li {...props}>
                    {typeof option === "string" ? option : option.label}
                  </li>
                )}
                freeSolo
                renderInput={(params) => (
                  <TextField
                    {...params}
                    label="Select or Add new"
                    variant="outlined"
                    className="auto-complete"
                  />
                )}
              />
            </Box>
          </Box>
          <Box>
            <Box Box flex="0 0 150px"></Box>
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
    </div>
  );
}
