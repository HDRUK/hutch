import {
  Heading,
  VStack,
  Button,
  useDisclosure,
  Input,
  HStack,
  Text,
  InputGroup,
  InputLeftElement,
  Stack,
  Box,
} from "@chakra-ui/react";
import { useSortingAndFiltering } from "helpers/hooks/useSortingAndFiltering";
import { useActivitySourceList } from "api/activitysources";
import { ActivitySourceSummary } from "components/activitysources/ActivitySourceSummary";
import { useState } from "react";
import { useBackendApi } from "contexts/BackendApi";
import { DeleteModal } from "components/DeleteModal";
import { useNavigate } from "react-router-dom";
import { FaPlus, FaSearch, FaInfo, FaInfoCircle } from "react-icons/fa";

export const UserHome = () => {
  const { isOpen, onOpen, onClose } = useDisclosure();
  const [selectedActivitySource, setSelectedActivitySource] = useState();
  const [isLoading, setIsLoading] = useState();
  const { activitysource } = useBackendApi();
  const { data, mutate } = useActivitySourceList();
  const navigate = useNavigate();
  const {
    sorting,
    setSorting,
    onSort: handleSort,
    filter,
    setFilter,
    outputList,
  } = useSortingAndFiltering(data, "displayName", {
    initialSort: {
      key: "displayName",
    },
    sorters: {
      displayName: {
        sorter: (asc) => (a, b) =>
          asc ? a.localeCompare(b) : b.localeCompare(a),
      },
    },
  });

  const onDeleteSource = async () => {
    setIsLoading(true);
    await activitysource.delete({ id: selectedActivitySource.id });
    await mutate();
    onClose();
    setIsLoading(false);
  };
  const onClickDelete = (activitySource) => {
    setSelectedActivitySource(activitySource);
    onOpen();
  };
  return (
    <Stack px={8} w="100%" spacing={4} p={4} alignItems="center">
      {data.length > 0 ? (
        <Stack w="100%" spacing={4}>
          <HStack
            maxW={"800"}
            w={"100%"}
            alignSelf={"center"}
            borderRadius={"10px"}
          >
            <InputGroup>
              <InputLeftElement
                pointerEvents="none"
                children={<FaSearch color="gray.300" />}
              />
              <Input
                size={"md"}
                placeholder="Search Activity Sources"
                onChange={(e) => setFilter(e.target.value)}
              />
            </InputGroup>
            <Button
              onClick={() => navigate("/activitysources/new")}
              colorScheme="green"
              leftIcon={<FaPlus />}
            >
              <Text
                textTransform={"uppercase"}
                fontWeight={700}
                fontSize={"sm"}
                letterSpacing={1.1}
              >
                New
              </Text>
            </Button>
          </HStack>

          {outputList.map((item, index) => (
            <>
              <ActivitySourceSummary
                key={index}
                href={`/activitysources/${item.id}`}
                onDelete={() => onClickDelete(item)}
                title={item.displayName}
                sourceURL={item.host}
                collectionId={item.resourceId}
              />
            </>
          ))}
        </Stack>
      ) : (
        <div>
          <Box textAlign="center" py={10} px={6}>
            <FaInfoCircle
              fontSize={"2em"}
              color="dodgerblue"
              style={{ display: "inline" }}
            />
            <Heading as="h2" size="xl" mb={2}>
              No Activity Sources found.
            </Heading>
            <Button
              onClick={() => navigate("/activitysources/new")}
              colorScheme={"green"}
              leftIcon={<FaPlus />}
              width={"225"}
            >
              <Text
                textTransform={"uppercase"}
                fontWeight={700}
                fontSize={"sm"}
                letterSpacing={1.1}
              >
                Create Activity Source
              </Text>
            </Button>
          </Box>
        </div>
      )}
      <DeleteModal
        title={`Delete Activity Source?`}
        body={
          <VStack>
            <Text>Are you sure you want to delete this activity source:</Text>
            <Text fontWeight="bold">{selectedActivitySource?.displayName}</Text>
            <Text>You will not be able to reverse this!</Text>
          </VStack>
        }
        isOpen={isOpen}
        onClose={onClose}
        onDelete={onDeleteSource}
        isLoading={isLoading}
      />
    </Stack>
  );
};
