const API_BASE_URL =
  process.env.REACT_APP_API_URL || "http://localhost:5000/api";

const API_ROUTES = {
  applications: `${API_BASE_URL}/applications`,
  applicationById: (id) => `${API_BASE_URL}/applications/${id}`,
  applicationEvents: (appId) => `${API_BASE_URL}/applications/${appId}/events`,
  eventById: (appId, eventId) =>
    `${API_BASE_URL}/applications/${appId}/events/${eventId}`,
  jobTitles: `${API_BASE_URL}/jobtitles`,
  organizations: `${API_BASE_URL}/organizations`,
  sources: `${API_BASE_URL}/sources`,
  workenvironments: `${API_BASE_URL}/workenvironments`,
};

export default API_ROUTES;
