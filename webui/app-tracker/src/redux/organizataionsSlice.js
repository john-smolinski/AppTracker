import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";

const API_BASE_URL = "http://localhost:5000/api/organizations";

// fetch Organizations
export const fetchOrganizations = createAsyncThunk(
  "organizations/fetchOrganizations",
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

// Organization Slice
const organizationsSlice = createSlice({
  name: "organizations",
  initialState: {
    orgs: [],
    loading: false,
    error: null,
    isServerError: false,
  },
  reducers: {},
  extraReducers: (builder) => {
    builder
      // Fetch Organizations
      .addCase(fetchOrganizations.pending, (state) => {
        state.loading = true;
        state.error = null;
        state.isServerError = false;
      })
      .addCase(fetchOrganizations.fulfilled, (state, action) => {
        state.loading = false;
        state.titles = action.payload;
      })
      .addCase(fetchOrganizations.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload?.message || "An error occurred";
        state.isServerError = action.payload?.status === 500;
      });
  },
});

export default organizationsSlice.reducer;
