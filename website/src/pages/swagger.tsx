import Layout from "@theme/Layout";
import React from "react";
import SwaggerUI from "swagger-ui-react";
import "swagger-ui-react/swagger-ui.css";

const SwaggerPage = () => (
  <Layout>
    <SwaggerUI url="/hutch/swagger.json" />
  </Layout>
);

export default SwaggerPage;
