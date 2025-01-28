import React, { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Autocomplete, TextField } from "@mui/material";
import Menu from "../../Menu/Menu";
import "./AddApplication.css";

export default function AddApplication() {
  useEffect(() => {
    console.log(orgs.length);
  });

  // access autocomplete values from the Redux state
  const sources = useSelector((state) => state.appSources.sources);
  const orgs = useSelector((state) => state.organizations.orgs);
  const jobTitles = useSelector((state) => state.jobTitles.titles);

  const [selectedSource, setSelectedSource] = useState("");
  const [selectedOrganization, setSelectedOrganization] = useState("");
  const [selectedTitle, setSelectedTitle] = useState("");

  // handle change events for different controls
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
        label: `Add {params.inputValue}`,
      });
    }
    return filtered;
  };

  return (
    <div className="app">
      <Menu />
      <h2>Add Application</h2>
      {/* outter boundry */}
      <div className="add-application">
        {/* development prints */}
        <div className="add-application">
          <ul>
            <li>
              Source: {selectedSource || "null"} | {sources.length}
            </li>
            <li>
              Organization {selectedOrganization || "null"} | {orgs.length}
            </li>
            <li>
              Title: {selectedTitle || "null"} | {jobTitles.length}{" "}
            </li>
          </ul>
        </div>
        <div className="add-application">
          <div>
            <label>Date:</label> Date Picker
          </div>
          {/* select/add source control */}
          <div className="add-application">
            Source:
            <Autocomplete
              sx={{ width: 300 }}
              value={selectedSource}
              onChange={handleChange(setSelectedSource)}
              filterOptions={filteredOptions}
              selectOnFocus
              clearOnBlur
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
          </div>
          {/* insert organization here */}
          <div className="add-application">
            Organization:
            <Autocomplete
              sx={{ width: 300 }}
              value={selectedOrganization}
              onChange={handleChange(setSelectedOrganization)}
              filterOptions={filteredOptions}
              selectOnFocus
              clearOnBlur
              handleHomeEndKeys
              options={orgs.map((x) => x.name)}
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
          </div>
          {/* select/add job title control */}
          <div className="add-application">
            Job title:
            <Autocomplete
              sx={{ width: 300 }}
              value={selectedTitle}
              onChange={handleChange(setSelectedTitle)}
              filterOptions={filteredOptions}
              selectOnFocus
              clearOnBlur
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
          </div>

          {/* insert work environment here */}
        </div>
      </div>
    </div>
  );
}
