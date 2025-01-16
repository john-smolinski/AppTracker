import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";

// temporary hard-coded API base URL for development purposes, targets containerized backend
const API_BASE_URL = "http://localhost:5000/api/applications";

// Thunks for async operations
export const fetchApplications = createAsyncThunk(
  "applications/fetchApplications",
  async (_, { rejectWithValue }) => {
    try {
      const response = await axios.get(API_BASE_URL);
      return response.data;
    } catch (error) {
      return rejectWithValue(
        error.message?.data || "error fetching applications"
      );
    }
  }
);

export const postApplication = createAsyncThunk(
  "applications/postApplication",
  async (newApplication, { rejectWithValue }) => {
    try {
      const response = await axios.post(API_BASE_URL, newApplication);
      return response.data;
    } catch (error) {
      return rejectWithValue(
        error.message?.data || "error posting application"
      );
    }
  }
);

// Slice for application state
const applicationSlice = createSlice({
  name: "applications",
  initialState: {
    itmes: [],
    loading: false,
    error: null,
  },
  reducers: {},
  extraReducers: (builder) => {
    builder
      // Fetch applications
      .addCase(fetchApplications.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchApplications.fulfilled, (state, action) => {
        state.loading = false;
        state.items = action.payload;
      })
      .addCase(fetchApplications.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;

        // check for server error and set flag
        if (action.payload?.status === 500) {
          state.isServerError = true;
        }
      })
      // POST application
      .addCase(postApplication.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(postApplication.fulfilled, (state, action) => {
        state.loading = false;
        state.items.push(action.payload);
      })
      .addCase(postApplication.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;

        // check for server error and set flag
        if (action.payload?.status === 500) {
          state.isServerError = true;
        }
      });
  },
});

export default applicationSlice.reducer;
