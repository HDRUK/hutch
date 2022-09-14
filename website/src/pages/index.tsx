import React from "react";
import Layout from "@theme/Layout";
import { Heading, Stack, Text, SimpleGrid, VStack } from "@chakra-ui/react";
import { Features } from "@site/src/components/homepage/Features";

const HeroBanner = () => {
  const responsiveChildWidths = { sm: "100%", md: "70%" };
  return (
    <Stack
      bgGradient="radial(circle 1000px at top left, cyan.600, blue.900)"
      py={10}
      alignItems="center"
      color="white"
      textShadow="2px 2px 2px rgba(0,0,0,.8)"
    >
      <Heading size="4xl">
        <Text>📤🐇 Hutch</Text>
      </Heading>

      <Heading
        size="2xl"
        p={5}
        width={responsiveChildWidths}
        textAlign="center"
        color="purple.200"
      >
        <Text as="span">Federated Data Discovery</Text>
      </Heading>
    </Stack>
  );
};

const LinkCardSection = () => (
  <VStack p={5} w="100%" align="stretch">
    <Heading fontWeight="medium">📚 Documentation</Heading>
    <SimpleGrid p={8} columns={{ sm: 1, md: 2 }} spacing={8}>
      {/* <DirectoryLinkCard />
      <ApiLinkCard />
      <InstallationLinkCard />
      <DeveloperLinkCard /> */}
    </SimpleGrid>
  </VStack>
);

const Index = () => {
  return (
    <Layout>
      <Stack>
        <HeroBanner />

        <Features />

        {/* <LinkCardSection /> */}
      </Stack>
    </Layout>
  );
};

export default Index;
