import React from "react";
import { Autocomplete, TextField } from "@mui/material";

const AutoCompleteField = ({
  resetKey,
  sx,
  value,
  onChange,
  onBlur,
  filteredOptions,
  options,
  getOptionLabel,
  renderOption,
  freeSolo,
  label,
}) => {
  return (
    <Autocomplete
      key={resetKey}
      sx={sx}
      value={value}
      onChange={onChange}
      onBlur={onBlur}
      filterOptions={filteredOptions}
      selectOnFocus
      handleHomeEndKeys
      options={options}
      getOptionLabel={getOptionLabel}
      renderOption={renderOption}
      freeSolo={freeSolo}
      renderInput={(params) => (
        <TextField
          {...params}
          label={label}
          variant="outlined"
          className="auto-complete"
        />
      )}
    />
  );
};

export default AutoCompleteField;
