import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";

const API_BASE_URL = "http://localhost:5000/api/sources";

// fetch Sources
export const fetchSources = createAsyncThunk(
  "sources/fetchSources",
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

// Source Slice
const sourceSlice = createSlice({
  name: "sources",
  initialState: {
    items: [],
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
        state.items = action.payload;
      })
      .addCase(fetchSources.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload?.message || "An error occurred";
        state.isServerError = action.payload?.status === 500;
      });
  },
});

export default sourceSlice.reducer;
