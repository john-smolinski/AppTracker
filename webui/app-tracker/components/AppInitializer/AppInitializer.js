import { useEffect } from "react";
import { useDispatch } from "react-redux";
import { fetchApplications } from "@/redux/applicationsSlice";
import { fetchSources } from "@/redux/sourcesSlice";
import { fetchOrganizations } from "@/redux/organizataionsSlice";
import { fetchJobTitles } from "@/redux/jobTitlesSlice";
import { fetchWorkEnvironments } from "@/redux/workEnvironmentsSlice";

export default function AppInitializer() {
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(fetchApplications());
    dispatch(fetchSources());
    dispatch(fetchOrganizations());
    dispatch(fetchJobTitles());
    dispatch(fetchWorkEnvironments());
  }, [dispatch]);

  return null;
}
