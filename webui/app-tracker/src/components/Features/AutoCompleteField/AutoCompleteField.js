import React from "react";
import {
  Autocomplete,
  TextField,
  Box,
  FormControl,
  FormLabel,
} from "@mui/material";

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
    <Box display="flex" alignItems="center">
      <Box flex="0 0 150px">
        <FormControl>
          <FormLabel>{label}:</FormLabel>
        </FormControl>
      </Box>
      <Box flex="1">
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
              label={`Select or Add ${label}`}
              variant="outlined"
              className="auto-complete"
            />
          )}
        />
      </Box>
    </Box>
  );
};

export default AutoCompleteField;
