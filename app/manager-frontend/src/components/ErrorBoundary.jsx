import { Container, Text } from "@chakra-ui/react";
import { cloneElement, Component } from "react";
import { useLocation } from "react-router-dom";
import { TitledAlert } from "./TitledAlert";
import { isEqual } from "lodash-es";
import { useTranslation } from "react-i18next";

const DefaultFallback = ({ tKey }) => {
  const { t } = useTranslation();
  return (
    <Container my={16}>
      <TitledAlert status="error" title={t("feedback.error_title")}>
        <Text>{t(tKey ?? "feedback.error")}</Text>
      </TitledAlert>
    </Container>
  );
};

const LocationProvider = ({ children }) => {
  const location = useLocation();
  return children(location);
};

class LocationAwareErrorBoundary extends Component {
  state = { hasError: false, error: null };
  static getDerivedStateFromError(error) {
    return {
      hasError: true,
      error,
    };
  }

  componentDidCatch(error, info) {
    console.error("ErrorBoundary caught an error", error, info);
  }

  componentDidUpdate(prevProps) {
    if (this.props.resetOnPropChanges && !isEqual(prevProps, this.props)) {
      this.setState({
        hasError: false,
      });
    }
    if (prevProps.location.key !== this.props.location.key) {
      this.setState({
        hasError: false,
      });
    }
  }

  render() {
    if (this.state.hasError) {
      return cloneElement(this.props.fallback ?? <DefaultFallback />, {
        __BoundaryError: this.state.error,
      });
    }
    return this.props.children;
  }
}

export class ErrorBoundary extends Component {
  render() {
    return (
      <LocationProvider>
        {(location) => (
          <LocationAwareErrorBoundary location={location} {...this.props} />
        )}
      </LocationProvider>
    );
  }
}
