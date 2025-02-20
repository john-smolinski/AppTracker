import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";
import API_ROUTES from "../config/apiConfig";

// fetch Sources
export const fetchSources = createAsyncThunk(
  "sources/fetchSources",
  async (_, { rejectWithValue }) => {
    try {
      const response = await axios.get(API_ROUTES.sources);
      return response.data;
    } catch (error) {
      return rejectWithValue({
        status: error.response?.status,
        message: error.response?.data || error.message,
      });
    }
  }
);

// Source Slice
const sourcesSlice = createSlice({
  name: "sources",
  initialState: {
    sources: [],
    loading: false,
    error: null,
    isServerError: false,
  },
  reducers: {},
  extraReducers: (builder) => {
    builder
      // Fetch Sources
      .addCase(fetchSources.pending, (state) => {
        state.loading = true;
        state.error = null;
        state.isServerError = false;
      })
      .addCase(fetchSources.fulfilled, (state, action) => {
        state.loading = false;
        state.sources = action.payload;
      })
      .addCase(fetchSources.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload?.message || "An error occurred";
        state.isServerError = action.payload?.status === 500;
      });
  },
});

export default sourcesSlice.reducer;
