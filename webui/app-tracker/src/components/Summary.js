import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchApplications } from "../redux/applicationSlice";

export default function Summary() {
  const dispatch = useDispatch();
  const { items, loading, error, isServerError } = useSelector(
    (state) => state.applications
  );

  useEffect(() => {
    dispatch(fetchApplications());
  }, [dispatch]);

  if (loading) return <p>Loading...</p>;
  if (isServerError)
    return <h3>Server error occurred. Please try again later.</h3>;
  if (error)
    return (
      <div>
        <h1>Error:</h1> <p> {error.message}</p>
      </div>
    );
  return (
    <div>
      <h3>Total Applications {items.length}</h3>
      <div></div>
    </div>
  );
}
