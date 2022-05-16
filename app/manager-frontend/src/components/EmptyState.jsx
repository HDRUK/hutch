import { FaQuestion } from "react-icons/fa";
import { Flex, Button, Icon, Heading } from "@chakra-ui/react";
import { useTranslation } from "react-i18next";

export const EmptyState = ({ splash = FaQuestion, message, callToAction }) => {
  const { t } = useTranslation();
  message = message ?? t("feedback.emptyState");
  return (
    <Flex direction="column" w="100%" align="center" justify="center">
      <Flex
        w="20%"
        color="cyan.500"
        borderColor="cyan.500"
        borderWidth={2}
        borderRadius={15}
        p={10}
      >
        <Icon as={splash} boxSize="100%" />
      </Flex>
      <Heading as="h1" size="xl" m={8}>
        {message}
      </Heading>
      {callToAction && (
        <Button colorScheme="blue" size="lg" onClick={callToAction.onClick}>
          {callToAction.label}
        </Button>
      )}
    </Flex>
  );
};
