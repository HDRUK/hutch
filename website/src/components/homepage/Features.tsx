import React from "react";
import { Divider, Flex, HStack, Link, Text, VStack } from "@chakra-ui/react";

export const Features = () => (
  <HStack p={5} justify="center" align="start" spacing={150}>
    <OpenSource />
    <Secure />
    <Hdr />
  </HStack>
);

const Feature = ({ graphic, children }) => (
  <VStack w="10em" textAlign="center">
    <Flex align="center" justify="center" width="100%" height="130px">
      {graphic}
    </Flex>

    <Text>{children}</Text>
  </VStack>
);

const OpenSource = () => (
  <Feature graphic={<Text fontSize={72}>❤️</Text>}>
    Hutch is Open Source software under the permissive MIT license.
  </Feature>
);

const Hdr = () => {
  const Svg = require("@site/static/img/hdruk_logo.svg").default;
  return (
    <Feature
      graphic={
        <Link href="https://www.hdruk.ac.uk/" isExternal title="HDR UK">
          <Svg role="img" />
        </Link>
      }
    >
      Hutch is being developed for use in HDR UK's{" "}
      <Link
        href="https://www.healthdatagateway.org/about/cohort-discovery"
        isExternal
      >
        Cohort Discovery
      </Link>{" "}
      toolset.
    </Feature>
  );
};

const Secure = () => (
  <Feature graphic={<Text fontSize={72}>🔒</Text>}>
    Make your data discoverable{" "}
    <Text as="span" color="green.300" fontWeight="medium">
      safely
    </Text>{" "}
    and{" "}
    <Text as="span" color="yellow.200" fontWeight="medium">
      securely
    </Text>
    , without directly sharing it.
  </Feature>
);
