import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";

const API_BASE_URL = "http://localhost:5000/api/applications";

// Fetch Applications
export const fetchApplications = createAsyncThunk(
  "applications/fetchApplications",
  async (_, { rejectWithValue }) => {
    try {
      const response = await axios.get(API_BASE_URL);
      return response.data;
    } catch (error) {
      return rejectWithValue({
        status: error.response?.status,
        message: error.response?.data || error.message,
      });
    }
  }
);

// Post Application
export const postApplication = createAsyncThunk(
  "applications/postApplication",
  async (newApplication, { rejectWithValue }) => {
    try {
      const response = await axios.post(API_BASE_URL, newApplication);
      return response.data;
    } catch (error) {
      return rejectWithValue({
        status: error.response?.status,
        message: error.response?.data || error.message,
      });
    }
  }
);

// Application Slice
const applicationsSlice = createSlice({
  name: "applications",
  initialState: {
    apps: [],
    loading: false,
    error: null,
    isServerError: false,
  },
  reducers: {},
  extraReducers: (builder) => {
    builder
      // Fetch Applications
      .addCase(fetchApplications.pending, (state) => {
        state.loading = true;
        state.error = null;
        state.isServerError = false;
      })
      .addCase(fetchApplications.fulfilled, (state, action) => {
        state.loading = false;
        state.apps = action.payload;
      })
      .addCase(fetchApplications.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload?.message || "An error occurred";
        state.isServerError = action.payload?.status === 500;
      })
      // Post Application
      .addCase(postApplication.pending, (state) => {
        state.loading = true;
        state.error = null;
        state.isServerError = false;
      })
      .addCase(postApplication.fulfilled, (state, action) => {
        state.loading = false;
        state.apps.push(action.payload);
      })
      .addCase(postApplication.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload?.message || "An error occurred";
        state.isServerError = action.payload?.status === 500;
      });
  },
});

export default applicationsSlice.reducer;
