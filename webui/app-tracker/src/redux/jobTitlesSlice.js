import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";

const API_BASE_URL = "http://localhost:5000/api/jobtitles";

// fetch Job Titles
export const fetchJobTitles = createAsyncThunk(
  "jobtitles/fetchJobTitles",
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

// Job Title Slice
const jobTitlesSlice = createSlice({
  name: "jobTitles",
  initialState: {
    titles: [],
    loading: false,
    error: null,
    isServerError: false,
  },
  reducers: {},
  extraReducers: (builder) => {
    builder
      // Fetch JobTitles
      .addCase(fetchJobTitles.pending, (state) => {
        state.loading = true;
        state.error = null;
        state.isServerError = false;
      })
      .addCase(fetchJobTitles.fulfilled, (state, action) => {
        state.loading = false;
        state.titles = action.payload;
      })
      .addCase(fetchJobTitles.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload?.message || "An error occurred";
        state.isServerError = action.payload?.status === 500;
      });
  },
});

export default jobTitlesSlice.reducer;
